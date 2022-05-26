using System.Net;

namespace Transaction.Application.Exceptions;

public class InsufficientBalanceException : AppException
{
    public InsufficientBalanceException()
        : base($"This operation can not be performed due to insufficient balance in the account.")
    { }
}