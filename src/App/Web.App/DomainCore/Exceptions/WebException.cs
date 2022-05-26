using System;
using System.Net;


namespace DomainCore.Exceptions
{
    [Serializable]
   
    public abstract class WebException : Exception
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;
        protected WebException(string message) : base(message)
        {
        }

        protected WebException(string message, HttpStatusCode httpStatusCode) : base(message)
        {
            StatusCode = httpStatusCode;
        }
        protected WebException(Exception exception, string message) : base(message, exception)
        {
        }
    }
}