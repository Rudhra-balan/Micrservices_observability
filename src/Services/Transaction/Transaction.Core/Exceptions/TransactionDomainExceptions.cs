
using System.Net;

namespace AccountSummary.Core.Exceptions;

public abstract class TransactionDomainExceptions : Exception
{
 
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;
    protected TransactionDomainExceptions(string message) : base(message)
    {
    }

    protected TransactionDomainExceptions(string message, HttpStatusCode httpStatusCode) : base(message)
    {
        StatusCode = httpStatusCode;
    }
    protected TransactionDomainExceptions(Exception exception, string message) : base(message, exception)
    {
    }
}