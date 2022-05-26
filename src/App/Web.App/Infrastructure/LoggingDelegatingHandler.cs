
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class LoggingDelegatingHandler : DelegatingHandler
    {
        private readonly ILogger<LoggingDelegatingHandler> _logger;

        public LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger)
        {
            _logger = logger;
        }


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (_logger.BeginScope("Sending request to {Url}", request.RequestUri))
            {

                try
                {
                    request.Headers.TryAddWithoutValidation("X-Correlation-Id", Guid.NewGuid().ToString());

                    long start = Stopwatch.GetTimestamp();

                  

                    var response = await base.SendAsync(request, cancellationToken);

                    long end = Stopwatch.GetTimestamp();

                    TimeSpan elapsedSpan = new TimeSpan((end - start));
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"Received a success response from {response.RequestMessage.RequestUri}  at {elapsedSpan.Milliseconds} Milliseconds");
                    }
                    else
                    {
                        if(response.RequestMessage != null)
                        _logger.LogWarning($"Received a non-success status code { (int)response.StatusCode} from {response.RequestMessage.RequestUri} at at {elapsedSpan.Milliseconds} Milliseconds");
                       
                    }

                    return response;
                }
                catch (HttpRequestException ex)
                    when (ex.InnerException is SocketException se && se.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    var hostWithPort = request.RequestUri.IsDefaultPort
                        ? request.RequestUri.DnsSafeHost
                        : $"{request.RequestUri.DnsSafeHost}:{request.RequestUri.Port}";

                    _logger.LogCritical(ex, $"Unable to connect to {hostWithPort}. Please check the configuration to ensure the correct URL for the service has been configured.");
                }

                return new HttpResponseMessage(HttpStatusCode.BadGateway)
                {
                    RequestMessage = request
                };

            }
        }
    }
}
