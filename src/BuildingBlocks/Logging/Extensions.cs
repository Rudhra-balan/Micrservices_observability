using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Logging.Options;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewRelic.LogEnrichers;
using NewRelic.LogEnrichers.Serilog;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace BuildingBlocks.Logging;

public static class Extensions
{
    private const string LoggerSectionName = "logger";
    private const string AppSectionName = "app";

    public static IHostBuilder UseLogging(this IHostBuilder builder, Action<LoggerConfiguration> configure = null)
    {
        return builder.UseSerilog((context, services, loggerConfiguration) =>
        {
          
            var loggerOptions = context.Configuration.GetOptions<LoggerOptions>(LoggerSectionName);
            var appOptions = context.Configuration.GetOptions<Types.Options.AppOptions>(AppSectionName);
            
            MapOptions(loggerOptions, appOptions, loggerConfiguration, context.HostingEnvironment.EnvironmentName);
            builder.ConfigureServices(services => services.AddSingleton(
                     serviceProvider =>
                     {
                         if (!string.IsNullOrWhiteSpace(context.Configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY")))
                         {
                             loggerConfiguration.WriteTo.ApplicationInsights(
                                 serviceProvider.GetRequiredService<TelemetryConfiguration>(),
                                 TelemetryConverter.Traces);
                         }
                         return loggerConfiguration;
                     }));
            configure?.Invoke(loggerConfiguration);

        });
    }

    
    private static void MapOptions(LoggerOptions loggerOptions, Types.Options.AppOptions appOptions,
        LoggerConfiguration loggerConfiguration, string environmentName)
    {
        var level = GetLogEventLevel(loggerOptions.Level);

        loggerConfiguration.Enrich.FromLogContext()
            .MinimumLevel.Is(level)

            .Enrich.WithProperty("Environment", environmentName)
            .Enrich.WithProperty("Application", appOptions.Service)
            .Enrich.WithProperty("Instance", appOptions.Instance)
            .Enrich.WithProperty("Version", appOptions.Version)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithThreadName()
            .Enrich.WithThreadId()
            .Enrich.WithNewRelicLogsInContext();
          



        foreach (var (key, value) in loggerOptions.Tags ?? new Dictionary<string, object>())
            loggerConfiguration.Enrich.WithProperty(key, value);

        foreach (var (key, value) in loggerOptions.MinimumLevelOverrides ?? new Dictionary<string, string>())
        {
            var logLevel = GetLogEventLevel(value);
            loggerConfiguration.MinimumLevel.Override(key, logLevel);
        }

        loggerOptions.ExcludePaths?.ToList().ForEach(p => loggerConfiguration.Filter
            .ByExcluding(Matching.WithProperty<string>("RequestPath", n => n.EndsWith(p))));

        loggerOptions.ExcludeProperties?.ToList().ForEach(p => loggerConfiguration.Filter
            .ByExcluding(Matching.WithProperty(p)));

       

        Configure(loggerConfiguration, level, loggerOptions);
    }
    private static LoggerConfiguration AddApplicationInsightsLogging(this LoggerConfiguration loggerConfiguration, IServiceProvider services, IConfiguration configuration)
    {
        if (!string.IsNullOrWhiteSpace(configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY")))
        {
            loggerConfiguration.WriteTo.ApplicationInsights(
                services.GetRequiredService<TelemetryConfiguration>(),
                TelemetryConverter.Traces);
        }

        return loggerConfiguration;
    }
    private static void Configure(LoggerConfiguration loggerConfiguration, LogEventLevel level,
        LoggerOptions options)
    {
        var consoleOptions = options.Console ?? new ConsoleOptions();
        var fileOptions = options.File ?? new Options.FileOptions();
        var elkOptions = options.Elk ?? new ElkOptions();
        var seqOptions = options.Seq ?? new SeqOptions();

        if (consoleOptions.Enabled) loggerConfiguration.WriteTo.Console(new RenderedCompactJsonFormatter());

        if (fileOptions.Enabled)
        {
            var path = string.IsNullOrWhiteSpace(fileOptions.Path) ? "logs/.txt" : fileOptions.Path;
            if (!Enum.TryParse<RollingInterval>(fileOptions.Interval, true, out var interval))
                interval = RollingInterval.Day;

         

           // loggerConfiguration.WriteTo.File(new ExceptionAsObjectJsonFormatter(renderMessage: true), path , rollingInterval: interval);
            loggerConfiguration.WriteTo.File(formatter: new NewRelicFormatter().WithPropertyMapping("ThreadId", NewRelicLoggingProperty.ThreadId)
                .WithPropertyMapping("ThreadName", NewRelicLoggingProperty.ThreadName), path , rollingInterval: interval);
        }

        if (elkOptions.Enabled)
            loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elkOptions.Url))
            {
                MinimumLogEventLevel = level,
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                IndexFormat = string.IsNullOrWhiteSpace(elkOptions.IndexFormat)
                    ? "logstash-{0:yyyy.MM.dd}"
                    : elkOptions.IndexFormat,
                ModifyConnectionSettings = connectionConfiguration =>
                    elkOptions.BasicAuthEnabled
                        ? connectionConfiguration.BasicAuthentication(elkOptions.Username, elkOptions.Password)
                        : connectionConfiguration
            });

        if (seqOptions.Enabled) loggerConfiguration.WriteTo.Seq(seqOptions.Url, apiKey: seqOptions.ApiKey);
    }

    private static LogEventLevel GetLogEventLevel(string level)
    {
        return Enum.TryParse<LogEventLevel>(level, true, out var logLevel)
            ? logLevel
            : LogEventLevel.Information;
    }


    public static IApplicationBuilder UserCorrelationContextLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationContextLoggingMiddleware>();

        return app;
    }
}