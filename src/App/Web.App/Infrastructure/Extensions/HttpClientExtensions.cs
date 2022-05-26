using DomainCore;
using DomainCore.Models.Response;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> ReadContentAs<T>(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                GlobalMetrics.RetryCounter.WithLabels(response.RequestMessage.Method.ToString(), response.RequestMessage.RequestUri.AbsolutePath).Set(0);

            if (!response.IsSuccessStatusCode)
                throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");

            var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return System.Text.Json.JsonSerializer.Deserialize<T>(dataAsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        public static async Task<HttpResponseMessage> PostAsJson<T>(this HttpClient httpClient, string url, T data)
        {
            try
            {

                var dataAsString = System.Text.Json.JsonSerializer.Serialize(data);
                var content = new StringContent(dataAsString);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var responseMessage = await httpClient.PostAsync(url, content);
                
                if(responseMessage.IsSuccessStatusCode)
                    GlobalMetrics.RetryCounter.WithLabels(responseMessage.RequestMessage.Method.ToString(), responseMessage.RequestMessage.RequestUri.AbsolutePath).Set(0);
               
                if (!responseMessage.IsSuccessStatusCode)
                    throw new ApplicationException($"Something went wrong calling the API: {responseMessage.ReasonPhrase}");
                return responseMessage;
            }
            catch (Exception ex)
            {
                Log.Error(ex,ex.GetWebExceptionMessages().StatusDescription);
                throw;

            }

        }
        public static async Task<WebClientResponse> PostAsJson<T>(this HttpClient httpClient, string url, object data)
        {
            HttpResponseMessage result= null;
            var webClientResponse = new WebClientResponse();
            try
            {
                var dataAsString = System.Text.Json.JsonSerializer.Serialize(data);
                var content = new StringContent(dataAsString);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                result = await httpClient.PostAsync(url, content);

                if (result.IsSuccessStatusCode)
                    GlobalMetrics.RetryCounter.WithLabels(result.RequestMessage.Method.ToString(), result.RequestMessage.RequestUri.AbsolutePath).Set(0);

                string responseDate = await result.Content.ReadAsStringAsync();

                var sourceObject = default(T);

                if (typeof(T) == typeof(bool))
                {
                    sourceObject = (T)Convert.ChangeType(data, typeof(bool));
                }
                else if (typeof(T) == typeof(string))
                {
                    sourceObject = (T)Convert.ChangeType(data, typeof(string));
                }
                else
                {
                    sourceObject = JsonConvert.DeserializeObject<T>(responseDate);
                }
                webClientResponse.ErrorId = (int)result.StatusCode;
                webClientResponse.SourceObject = sourceObject;

                if (result.StatusCode == HttpStatusCode.OK
                           || result.StatusCode == HttpStatusCode.NoContent)
                {
                    webClientResponse.IsOperationSuccess = true;
                    webClientResponse.ErrorDescription = result.ReasonPhrase;
                }
                else
                {
                    webClientResponse.IsOperationSuccess = false;

                    webClientResponse.ErrorDescription = result.ReasonPhrase;
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.GetWebExceptionMessages().StatusDescription);
                webClientResponse.IsOperationSuccess = false;
                if (result != null)
                    webClientResponse.ErrorDescription = result.ReasonPhrase;

                else
                {
                  var (status,descrption) =  ex.GetWebExceptionMessages();
                    webClientResponse.ErrorDescription = descrption;
                }
               
            }


            return webClientResponse;
        }

        public static Task<HttpResponseMessage> PutAsJson<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = System.Text.Json.JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return httpClient.PutAsync(url, content);
        }

        private static (int statusCode, string StatusDescription) GetWebExceptionMessages(this Exception ex)
        {
            if (ex is WebException exception)
                return (Convert.ToInt32(((HttpWebResponse)exception.Response).StatusCode),
                    ((HttpWebResponse)exception.Response).StatusDescription);

            if (ex.InnerException is WebException innerException)
            {
                if (ex.InnerException.GetBaseException() is SocketException &&
                    (HttpWebResponse)innerException.Response == null)
                {
                    return (HttpStatusCode.GatewayTimeout.ToInt(), " Service unavailable. Please contact administrator");
                }

                return (Convert.ToInt32(((HttpWebResponse)innerException.Response).StatusCode),
                    innerException.GetExceptionMessage());
            }


            if (!(ex is AggregateException aggregateException) || !aggregateException.InnerExceptions.Any())
            {
                return ((int)HttpStatusCode.InternalServerError, ex.GetExceptionMessage());
            }

            foreach (var innerEx in aggregateException.InnerExceptions)
            {
                if (!(innerEx is WebException innerex)) continue;
                return (Convert.ToInt32(((HttpWebResponse)innerex.Response).StatusCode), innerex.GetExceptionMessage());
            }

            return (0, string.Empty);
        }

    }
}
