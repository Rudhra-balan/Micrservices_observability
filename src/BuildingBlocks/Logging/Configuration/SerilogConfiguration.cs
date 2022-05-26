
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Enrichers;
using Serilog.Events;

namespace BuildingBlocks.Logging.Configuration
{
    public static class SerilogConfiguration
    {
        private const string Delimiter = " | ";
        public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
            (context, configuration) =>
            {
                configuration
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.FromLogContext()
                    
                    .Enrich.WithEnvironmentUserNameDelimited()
                    .Enrich.WithMachineNameDelimited()
                    .Enrich.WithPropertyDelimited("ClassName")
                    .Enrich.WithProperty("Version", "1.0.0")
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                    .ReadFrom.Configuration(context.Configuration);
            };

        public static Action<WebHostBuilderContext, LoggerConfiguration> ConfigureWebHost =>
            (context, configuration) =>
            {
                configuration
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.FromLogContext()
                
                    .Enrich.WithEnvironmentUserNameDelimited()
                    .Enrich.WithMachineNameDelimited()
                    .Enrich.WithPropertyDelimited("ClassName")
                    .Enrich.WithProperty("Version", "1.0.0")
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                    .ReadFrom.Configuration(context.Configuration);
            };

        public static IEnumerable<KeyValuePair<string, T>> PropertiesOfType<T>(this object obj)
        {
            return from p in obj.GetType().GetProperties()
                   where p.PropertyType == typeof(T)
                   select new KeyValuePair<string, T>(p.Name, (T)p.GetValue(obj));
        }


        public static LoggerConfiguration WithEnvironmentUserNameDelimited(
            this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            return enrichmentConfiguration.With(new DelimitedEnricher(new EnvironmentUserNameEnricher(),
                EnvironmentUserNameEnricher.EnvironmentUserNamePropertyName, Delimiter));
        }

        public static LoggerConfiguration WithMachineNameDelimited(
            this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            return enrichmentConfiguration.With(new DelimitedEnricher(new MachineNameEnricher(),
                MachineNameEnricher.MachineNamePropertyName, Delimiter));
        }

        public static LoggerConfiguration WithPropertyDelimited(
            this LoggerEnrichmentConfiguration enrichmentConfiguration, string propertyName)
        {
            return enrichmentConfiguration.With(new DelimitedEnricher(propertyName, Delimiter));
        }

    }

    public class DelimitedEnricher : ILogEventEnricher
    {
        private readonly string _delimiter;
        private readonly ILogEventEnricher _innerEnricher;
        private readonly string _innerPropertyName;

        public DelimitedEnricher(string innerPropertyName, string delimiter)
        {
            _innerPropertyName = innerPropertyName;
            _delimiter = delimiter;
        }

        public DelimitedEnricher(ILogEventEnricher innerEnricher, string innerPropertyName, string delimiter) : this(
            innerPropertyName, delimiter)
        {
            _innerEnricher = innerEnricher;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            _innerEnricher?.Enrich(logEvent, propertyFactory);

            if (!logEvent.Properties.TryGetValue(_innerPropertyName, out var eventPropertyValue)) return;
            var value = (eventPropertyValue as ScalarValue)?.Value as string;
            if (!string.IsNullOrEmpty(value))
                logEvent.AddPropertyIfAbsent(new LogEventProperty(_innerPropertyName + "Delimited",
                    new ScalarValue(value + _delimiter)));
        }
    }
}
