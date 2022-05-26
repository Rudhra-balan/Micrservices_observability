using System.Net;

namespace Transaction.Application.Exceptions;

public class InvalidAmountException : AppException
{
    public InvalidAmountException(decimal amount)
         : base($"This operation can not be performed for {amount} amount.",HttpStatusCode.Forbidden)
    { }
}