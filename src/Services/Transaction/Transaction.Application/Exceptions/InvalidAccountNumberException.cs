using System.Net;

namespace Transaction.Application.Exceptions;

public class InvalidAccountNumberException  : AppException
{
    public InvalidAccountNumberException(int accountNumber)
         : base($"This account {accountNumber} does not exist.", HttpStatusCode.BadRequest)
    { }
}