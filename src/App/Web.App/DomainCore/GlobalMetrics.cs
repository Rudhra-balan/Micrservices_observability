

using Prometheus;

namespace DomainCore
{
    public static class GlobalMetrics
    {
        public static Gauge RetryCounter = Metrics
                 .CreateGauge("resilience_http_retry_policy", "The total number of retries by this API.",
                  labelNames: new[] { "method", "endpoint" });
    }
}
