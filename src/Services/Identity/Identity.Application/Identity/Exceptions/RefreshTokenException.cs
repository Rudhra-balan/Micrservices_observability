using System.Net;

namespace Identity.Application.Identity.Exceptions;

public class RefreshTokenException : AppException
{
   
    public RefreshTokenException(string error) : base(error)
    {
    }
    public RefreshTokenException(string error, HttpStatusCode httpStatusCode) : base(error, httpStatusCode)
    {
    }
    public RefreshTokenException(string error, Exception exception) : base(exception, error)
    {

    }
}