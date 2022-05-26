using BuildingBlocks.Logging;
using BuildingBlocks.Metrics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Serilog;
using System;


namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {


            var builder = CreateHostBuilder(args).Build();
            var logger = builder.Services.GetService<ILogger<Program>>();
            try
            {
                logger.LogInformation("Starting web host");
                builder.Run();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Host unexpectedly terminated");
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
         Host.CreateDefaultBuilder(args).UseLogging()
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder.CaptureStartupErrors(true);
                 webBuilder.UseContentRoot(AppContext.BaseDirectory);
                 webBuilder.UseUrls("http://*:5000");
                 webBuilder.UseStartup<Startup>().ConfigureAppConfiguration((hostingContext, config) =>
                 {
                     config.AddJsonFile("web.appsettings.json", true, true);
                     config.AddJsonFile($"web.appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.log.json", true, true);
                     config.AddJsonFile($"web.appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);
                 });
                 webBuilder.UseAppMetrics();

             })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.ClearProviders();
                logging.AddSerilog();
                logging.AddApplicationInsights(
                           hostingContext.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
                logging.AddFilter<ApplicationInsightsLoggerProvider>(
                          typeof(Program).FullName, LogLevel.Trace);
                logging.AddFilter<ApplicationInsightsLoggerProvider>(
                            typeof(Startup).FullName, LogLevel.Trace);
                logging.Configure(options =>
            {
                options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId
                                                    | ActivityTrackingOptions.TraceId
                                                    | ActivityTrackingOptions.ParentId
                                                    | ActivityTrackingOptions.Baggage
                                                    | ActivityTrackingOptions.Tags;
            });


            });
    }

   
}