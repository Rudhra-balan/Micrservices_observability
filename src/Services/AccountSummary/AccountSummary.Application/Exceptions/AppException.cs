using System.Net;

namespace AccountSummary.Application.Exceptions;

public abstract class AppException : Exception
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;
    protected AppException(string message) : base(message)
    {
    }

    protected AppException(string message, HttpStatusCode httpStatusCode) : base(message)
    {
        StatusCode = httpStatusCode;
    }
    protected AppException(Exception exception, string message) : base(message, exception)
    {
    }
}