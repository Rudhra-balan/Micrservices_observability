

using Microsoft.Extensions.Logging;
using Prometheus;
using Prometheus.DotNetRuntime;
using System.Diagnostics;


namespace BuildingBlocks.Metrics.Helper
{
    public class DiagnosticsDelegatingHandler : DelegatingHandler
    {
        private readonly ILogger<DiagnosticsDelegatingHandler> _logger;
        private readonly Histogram _responseCpuUsage;
        private readonly Histogram _responseAllocatedUsage;
        private readonly Histogram _responseWorkingSetUsage;
        private readonly Histogram _responseRpsUsage;
        private readonly Counter _requestExceptionCountByMethod;
        private readonly Counter _requestCountByMethod;

        public DiagnosticsDelegatingHandler(ILogger<DiagnosticsDelegatingHandler> logger)
        {
            _logger = logger;


            _responseCpuUsage = Prometheus.Metrics
                    .CreateHistogram("request_cpu_usage",
                    "Monitor the cpu usage of your transaction microservice applications.", new HistogramConfiguration
                    {
                        Buckets = Histogram.ExponentialBuckets(0.01, 5, 10),
                        LabelNames = new[] { "method" }
                    });
            _responseAllocatedUsage = Prometheus.Metrics
                 .CreateHistogram("request_allocated_usage",
                 "The memory occupied by objects.", new HistogramConfiguration
                 {
                     Buckets = Histogram.ExponentialBuckets(0.01, 5, 10),
                     LabelNames = new[] { "method" }
                 });
            _responseWorkingSetUsage = Prometheus.Metrics
                 .CreateHistogram("request_WorkingSet_usage",
                 "The shared data includes the pages that contain all the instructions that the process executes.", new HistogramConfiguration
                 {
                     Buckets = Histogram.ExponentialBuckets(0.01, 5, 10),
                     LabelNames = new[] { "method" }
                 });

            _responseRpsUsage = Prometheus.Metrics
               .CreateHistogram("request_rps_usage",
               "Monitor the request per secound usage of your transaction microservice applications.", new HistogramConfiguration
               {
                   Buckets = Histogram.ExponentialBuckets(0.01, 5, 10),
                   LabelNames = new[] { "method" }
               });

            _requestExceptionCountByMethod = Prometheus.Metrics
                .CreateCounter("http_requests_exception_total", "Number of exception received, by HTTP method.",
                    new CounterConfiguration
                    {

                        LabelNames = new[] { "method", "path", "statuscode", "exception" }
                    });

            _requestCountByMethod = Prometheus.Metrics
                .CreateCounter("http_requests_total", "Number of requests received, by HTTP method.",
                    new CounterConfiguration
                    {

                        LabelNames = new[] { "method", "path", "statuscode" }
                    });
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

                //var dotNetRuntimeStatsBuilder =  DotNetRuntimeStatsBuilder
                //                                .Customize()
                //                                .WithContentionStats(CaptureLevel.Informational)
                //                                .WithJitStats(CaptureLevel.Informational)
                //                                .WithThreadPoolStats(CaptureLevel.Informational)
                //                                .WithGcStats(CaptureLevel.Informational)
                //                                .WithExceptionStats(CaptureLevel.Errors)
                //                                .StartCollecting(); 
            //var diagnosticSourceAdapter = DiagnosticSourceAdapter.StartListening();
            //var meterAdapter = MeterAdapter.StartListening();
            //var registration = EventCounterAdapter.StartListening();


            #region Variable Refresh for Every Request to calculate 

            var _process = Process.GetCurrentProcess();
            var _oldCPUTime = TimeSpan.Zero;
            var _lastMonitorTime = DateTime.UtcNow;
            var _lastRpsTime = DateTime.UtcNow;
            double _cpu = 0; double _rps = 0;
            var RefreshRate = TimeSpan.FromSeconds(1).TotalMilliseconds;


            #endregion

            var response = await base.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _requestExceptionCountByMethod.WithLabels(request.Method.ToString(), request.RequestUri.AbsolutePath, response.StatusCode.ToString(), response.ReasonPhrase).Inc();
            }
            _requestCountByMethod.WithLabels(request.Method.ToString(), request.RequestUri.AbsolutePath, response.StatusCode.ToString()).Inc();
            try
            {

                var now = DateTime.UtcNow;
                _process.Refresh();

                var cpuElapsedTime = now.Subtract(_lastMonitorTime).TotalMilliseconds;

                if (cpuElapsedTime > RefreshRate)
                {
                    var newCPUTime = _process.TotalProcessorTime;
                    var elapsedCPU = (newCPUTime - _oldCPUTime).TotalMilliseconds;
                    _cpu = elapsedCPU * 100 / Environment.ProcessorCount / cpuElapsedTime;

                    _lastMonitorTime = now;
                    _oldCPUTime = newCPUTime;
                }

                var rpsElapsedTime = now.Subtract(_lastRpsTime).TotalMilliseconds;
                if (rpsElapsedTime > RefreshRate)
                {
                    _rps = DiagnosticsConstant.Requests * 1000 / rpsElapsedTime;
                    Interlocked.Exchange(ref DiagnosticsConstant.Requests, 0);
                    _lastRpsTime = now;
                }

                _responseAllocatedUsage.WithLabels(request.RequestUri.AbsolutePath).Observe(GC.GetTotalMemory(false) / 1000000);
                _responseWorkingSetUsage.WithLabels(request.RequestUri.AbsolutePath).Observe(_process.WorkingSet64 / 1000000);
                _responseCpuUsage.WithLabels(request.RequestUri.AbsolutePath).Observe(Math.Round(_cpu));
                _responseRpsUsage.WithLabels(request.RequestUri.AbsolutePath).Observe(Math.Round(_rps / 1000));


                var diagnostics = new
                {
                    PID = _process.Id,

                    // The memory occupied by objects.
                    Allocated = GC.GetTotalMemory(false),

                    // The working set includes both shared and private data. The shared data includes the pages that contain all the 
                    // instructions that the process executes, including instructions in the process modules and the system libraries.
                    WorkingSet = _process.WorkingSet64,

                    // The value returned by this property represents the current size of memory used by the process, in bytes, that 
                    // cannot be shared with other processes.
                    PrivateBytes = _process.PrivateMemorySize64,

                    // The number of generation 0 collections
                    Gen0 = GC.CollectionCount(0),

                    // The number of generation 1 collections
                    Gen1 = GC.CollectionCount(1),

                    // The number of generation 2 collections
                    Gen2 = GC.CollectionCount(2),

                    CPU = _cpu,

                    RPS = _rps
                };

                _logger.LogInformation("Request Diagnostics Inforamtion {@diagnostics}", diagnostics);
            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                //registration.Dispose();
              //  dotNetRuntimeStatsBuilder.Dispose();
                //diagnosticSourceAdapter.Dispose();
                //meterAdapter.Dispose();

            }

            return response;
        }
    }

}
