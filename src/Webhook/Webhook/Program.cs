
using BuildingBlocks.Logging;
using BuildingBlocks.Metrics;
using Serilog;

namespace Webhook
{
    public  class Program
    {

        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args).Build();
            var logger = builder.Services.GetRequiredService<ILogger<Program>>();
            try
            {
                logger.LogInformation("Starting Webhook host");
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
                webBuilder.UseUrls("http://*:5006");
                webBuilder.UseStartup<Startup>().ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile(
                     $"webhook.appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.log.json",
                     true, true);
                    config.AddJsonFile("webhook.appsettings.json", true, true);
                    config.AddJsonFile($"webhook.appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true);
                });
                webBuilder.UseAppMetrics();
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