
using Prometheus;

namespace BuildingBlocks.Metrics
{
    public class MetricReporter
    {
     
        private readonly Counter _requestCounter;
        private readonly Histogram _responseTimeHistogram;

        public MetricReporter()
        {
           
            _requestCounter = Prometheus.Metrics
                .CreateCounter("total_requests", "The total number of requests serviced by this API."
                ,labelNames: new[] { "method", "endpoint" });

         
            _responseTimeHistogram = Prometheus.Metrics
                .CreateHistogram("request_duration_seconds",
                "The duration in seconds between the response to a request.", new HistogramConfiguration
                {
                    Buckets = Histogram.ExponentialBuckets(0.01, 2, 10),
                    LabelNames = new[] { "status_code", "method" }
                });

           
        }

        public void RegisterRequest(string method, string endPoint)
        {
            _requestCounter.WithLabels(method,endPoint).Inc();
        }

        public void RegisterResponseTime(int statusCode, string method, TimeSpan elapsed)
        {
            _responseTimeHistogram.Labels(statusCode.ToString(), method).Observe(elapsed.TotalSeconds);
        }
    }
}
