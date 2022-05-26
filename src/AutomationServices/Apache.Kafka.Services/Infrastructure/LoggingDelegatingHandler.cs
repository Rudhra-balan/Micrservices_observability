
using Microsoft.Extensions.Logging;
using SerilogTimings;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Client.App.Infrastructure
{
    public class LoggingDelegatingHandler : DelegatingHandler
    {
        private readonly ILogger<LoggingDelegatingHandler> logger;

        public LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger)
        {
            this.logger = logger;
        }


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                long start = Stopwatch.GetTimestamp();

                logger.LogInformation("Sending request to {Url} ", request.RequestUri);

                var response = await base.SendAsync(request, cancellationToken);

                long end = Stopwatch.GetTimestamp();

                TimeSpan elapsedSpan = new TimeSpan((end - start));
                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation("Received a success response from {Url}  at {0} Milliseconds", response.RequestMessage.RequestUri, elapsedSpan.Milliseconds);
                }
                else
                {
                    logger.LogWarning("Received a non-success status code {StatusCode} from {Url} at at {0} Milliseconds",
                        (int)response.StatusCode, response.RequestMessage.RequestUri, elapsedSpan.Milliseconds);
                }
                
                return response;
            }
            catch (HttpRequestException ex)
                when (ex.InnerException is SocketException se && se.SocketErrorCode == SocketError.ConnectionRefused)
            {
                var hostWithPort = request.RequestUri.IsDefaultPort
                    ? request.RequestUri.DnsSafeHost
                    : $"{request.RequestUri.DnsSafeHost}:{request.RequestUri.Port}";

                logger.LogCritical(ex, "Unable to connect to {Host}. Please check the " +
                                        "configuration to ensure the correct URL for the service " +
                                        "has been configured.", hostWithPort);
            }

            return new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                RequestMessage = request
            };

           
        }
    }
}
