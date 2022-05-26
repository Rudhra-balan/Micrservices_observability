using BuildingBlocks.Logging;
using Serilog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Metrics;

namespace Gateway.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args).Build();
            var logger = builder.Services.GetRequiredService<ILogger<Program>>();
           
            try
            {
                logger.LogInformation("Starting Gateway host");
                builder.Run();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Host unexpectedly terminated");
            }

        }

      

        private static IHostBuilder CreateHostBuilder(string[] args) =>
         Host.CreateDefaultBuilder(args).UseLogging()
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder.CaptureStartupErrors(true);
                 webBuilder.UseContentRoot(AppContext.BaseDirectory);
                 webBuilder.UseUrls("http://*:5004");
                 webBuilder.UseStartup<Startup>().ConfigureAppConfiguration((hostingContext, config) =>
                 {
                     config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                     config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
                          optional: true, reloadOnChange: true);
                     config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.configuration.json",
                           optional: true, reloadOnChange: true);

                     config.AddEnvironmentVariables();
                     webBuilder.UseAppMetrics();
                 });

             })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.ClearProviders();
                logging.AddSerilog();
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
