

using Microsoft.AspNetCore.Http;
using Prometheus;
using Prometheus.DotNetRuntime;
using System.Diagnostics;

namespace BuildingBlocks.Metrics
{
    public class ResponseTimeMiddleware
    {
        private readonly RequestDelegate _request;

        public ResponseTimeMiddleware(RequestDelegate request)
        {
            _request = request;
        }

        public async Task Invoke(HttpContext context, MetricReporter reporter)
        {
            
            

            if (context.Request.Path.Value.Contains("/swagger"))
            {
                await _request.Invoke(context);
                return;
            }
            if (context.Request.Path.Value == "/metrics")
            {
                await _request.Invoke(context);
                return;
            }
            var sw = Stopwatch.StartNew();

            try
            {
                await _request.Invoke(context);
            }
            finally
            {
                sw.Stop();

                reporter.RegisterRequest(context.Request.Method, context.Request.Path.Value);
                reporter.RegisterResponseTime(context.Response.StatusCode, context.Request.Path.Value, sw.Elapsed);
               
                

            }

        }

    }
}
