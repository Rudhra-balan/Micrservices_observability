using System.Net;

using BuildingBlocks;
using BuildingBlocks.Exception;
using Transaction.Application.Exceptions;

namespace Transaction.Infrastructure.Services;

public class ExceptionToResponseMapper : IExceptionToResponseMapper
{
    public ExceptionResponse Map(Exception exception)
    {
        return exception switch
        {
            InsufficientBalanceException ex => new ExceptionResponse()
            {
                StatusCode = ex.StatusCode,
                Response = ex.Message
            },

            InvalidAccountNumberException ex => new ExceptionResponse()
            {
                StatusCode = ex.StatusCode,
                Response = ex.Message
            },

            InvalidAmountException ex => new ExceptionResponse()
            {
                StatusCode = ex.StatusCode,
                Response = ex.Message
            },

            InvalidCurrencyException ex => new ExceptionResponse()
            {
                StatusCode = ex.StatusCode,
                Response = ex.Message
            },
            _ => new ExceptionResponse()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Response = exception.GetExceptionMessage()
            },
        };
    }
}