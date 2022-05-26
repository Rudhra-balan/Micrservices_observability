using System.Net;

namespace AccountSummary.Application.Exceptions;

public class InvalidAccountNumberException  : AppException
{
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;
    public InvalidAccountNumberException(int accountNumber)
         : base($"This account {accountNumber} does not exist.")
    { }
}