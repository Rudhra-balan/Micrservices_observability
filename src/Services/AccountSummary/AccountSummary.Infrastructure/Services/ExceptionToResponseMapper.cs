using System.Net;
using AccountSummary.Application.Exceptions;
using BuildingBlocks;
using BuildingBlocks.Exception;


namespace AccountSummary.Infrastructure.Services;

public class ExceptionToResponseMapper : IExceptionToResponseMapper
{
    public ExceptionResponse Map(Exception exception)
    {
        return exception switch
        {
            InvalidAccountNumberException ex => new ExceptionResponse()
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