

using BuildingBlocks;
using BuildingBlocks.Exception;
using Identity.Application.Identity.Exceptions;

namespace Identity.Infrastructure.Services;

public class ExceptionToResponseMapper : IExceptionToResponseMapper
{
    public ExceptionResponse Map(Exception exception)
    {
        return exception switch
        {
            AuthentioncationException ex => new ExceptionResponse()
            {
                StatusCode = ex.StatusCode,
                Response = ex.Message
            },
            RefreshTokenException ex => new ExceptionResponse()
            {
                StatusCode = ex.StatusCode,
                Response = ex.Message
            },
            RegisterIdentityUserException ex => new ExceptionResponse()
            {
                StatusCode = ex.StatusCode,
                Response = ex.Message
            },
            AppException ex => new ExceptionResponse()
            {
                StatusCode = ex.StatusCode,
                Response = ex.Message
            },
            _ =>  new ExceptionResponse()
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Response = exception.GetExceptionMessage()
            },
        };
    }
}