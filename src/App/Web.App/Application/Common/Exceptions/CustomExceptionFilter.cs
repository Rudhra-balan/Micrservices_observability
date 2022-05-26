using System;
using System.Net;
using DomainCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Application.Common.Exceptions
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;
        public CustomExceptionFilter(ILogger logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {

            _logger.Error("CustomExceptionFilter", context.Exception);

            int status;
            string message;

            var exceptionType = context?.Exception.GetType();
            if (exceptionType == typeof(UnauthorizedAccessException))
            {
                message = "Unauthorized Access";
                status = HttpStatusCode.Unauthorized.ToInt();
            }
            else if (exceptionType == typeof(NotImplementedException))
            {
                message = "Server error (nie)";
                status = HttpStatusCode.InternalServerError.ToInt();
            }
          
            else if (exceptionType == typeof(NullReferenceException))
            {
                message = "Server error (nre)";
                status = HttpStatusCode.InternalServerError.ToInt();
            }
            else
            {
                message = context?.Exception.Message;
                status = HttpStatusCode.InternalServerError.ToInt();
            }

            if (context != null)
            {
                context.ExceptionHandled = true;
                var response = context.HttpContext.Response;
                response.StatusCode = status;
                response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = message;
                response.ContentType = "application/json";

                // display stack trace if in development but always record it in log.
                var err = context.Exception.StackTrace;
                _logger.Debug(err);
                response.WriteAsync(err);
            }
        }
    }
}