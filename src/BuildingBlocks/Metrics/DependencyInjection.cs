

using BuildingBlocks.Metrics.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Prometheus.SystemMetrics;

namespace BuildingBlocks.Metrics
{
    public static class DependencyInjection
    {
        

        public static void UseAppMetrics(this IApplicationBuilder app)
        {
            app.UseMetricServer();
            app.UseHttpMetrics(options=>
            {
             
                options.AddCustomLabel("host", context => context.Request.Host.Host);
            });
           
        }
        public static void AddMetric(this IServiceCollection services)
        {
            services.AddTransient<MetricReporter>();
            services.AddSingleton<IMonitoringService, MonitoringService>();
            services.AddSystemMetrics();

        }
        public static IWebHostBuilder UseAppMetrics(this IWebHostBuilder webHostBuilder)
        {

            return webHostBuilder;
       
               
        }
    }
}