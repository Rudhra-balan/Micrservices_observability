
using Application.Common;
using BuildingBlocks;
using DomainCore;
using DomainCore.Helper.Constant;
using Infrastructure.HubService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Prometheus;
using Serilog;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace Infrastructure
{
    public static class Resilience
    {
        private static IHttpContextAccessor httpContextAccessor =>  ServiceActivator.IServiceProvider.GetRequiredService<IHttpContextAccessor>();
        private static IHubContext<EHubMessage> hubContext =>  ServiceActivator.IServiceProvider.GetRequiredService<IHubContext<EHubMessage>>();

       
        public static  IAsyncPolicy<HttpResponseMessage> HttpResilienceWraper => Policy.WrapAsync(fallbackPolicy, httpWaitAndRetryPolicy, GetCircuitBreakerPolicy, Timeout(60));

       
        private static IAsyncPolicy<HttpResponseMessage> httpWaitAndRetryPolicy =>
               Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
              .OrResult(r => HttpStatusCodesWorthRetrying.Contains(r.StatusCode))
              .Or<TimeoutRejectedException>()
              .WaitAndRetryAsync(
                      retryCount: 2,
                      sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                      onRetry: (exception, retryCount, context) =>
                      {
                          exception.Result?.Dispose();
                          GlobalMetrics.RetryCounter.WithLabels(exception.Result.RequestMessage.Method.ToString(), exception.Result.RequestMessage.RequestUri.AbsolutePath).Inc();
                          //Log.Error($"Retry {exception.Result.RequestMessage.Method} : {exception.Result.RequestMessage.RequestUri} , due to: {exception.Result.ReasonPhrase}.");
                          SendMessage($"Retry {exception.Result.RequestMessage.Method}:{exception.Result.RequestMessage.RequestUri} , due to: {exception.Result.ReasonPhrase}.");
                          
                      });

        private static IAsyncPolicy<HttpResponseMessage> fallbackPolicy =
            Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .FallbackAsync(FallbackAction, OnFallbackAsync);
      
        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy=>
             HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 7,
                    durationOfBreak: TimeSpan.FromSeconds(10)

                );
        
        private static IAsyncPolicy<HttpResponseMessage> Timeout(int seconds = 2) =>
          Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(seconds));

        // Handle both exceptions and return values in one policy
        private static HttpStatusCode[] HttpStatusCodesWorthRetrying = {
                   HttpStatusCode.RequestTimeout, // 408
                   HttpStatusCode.InternalServerError, // 500
                   HttpStatusCode.BadGateway, // 502
                   HttpStatusCode.ServiceUnavailable, // 503
                   HttpStatusCode.GatewayTimeout // 504
                };

        private static Task OnFallbackAsync(DelegateResult<HttpResponseMessage> response, Context context)
        {
       
            return Task.CompletedTask;
        }

        private static Task<HttpResponseMessage> FallbackAction(DelegateResult<HttpResponseMessage> responseToFailedRequest, Context context, CancellationToken cancellationToken)
        {
            SendMessage($"The fallback executed, the original error was {responseToFailedRequest.Result.ReasonPhrase}");
            //Log.Error($"The fallback executed, the original error was {responseToFailedRequest.Result.ReasonPhrase}");
           
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(responseToFailedRequest.Result.StatusCode)
            {
                
            Content = new StringContent($"The fallback executed, the original error was {responseToFailedRequest.Result.ReasonPhrase}")
            };
            return Task.FromResult(httpResponseMessage);
        }

        private static void SendMessage(string message)
        {
            hubContext.Clients.Group(httpContextAccessor.HttpContext.User.GetUserId().ToString()).SendAsync($"{SignalRMethod.ResilienceeNotification}", message).GetAwaiter().GetResult();
        }
    }
}
