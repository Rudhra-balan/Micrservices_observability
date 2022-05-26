
using System.Net;

namespace AccountSummary.Core.Exceptions;

public abstract class AccountSummaryExceptions : Exception
{
 
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;
    protected AccountSummaryExceptions(string message) : base(message)
    {
    }

    protected AccountSummaryExceptions(string message, HttpStatusCode httpStatusCode) : base(message)
    {
        StatusCode = httpStatusCode;
    }
    protected AccountSummaryExceptions(Exception exception, string message) : base(message, exception)
    {
    }
}