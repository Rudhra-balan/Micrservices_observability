using System.Net;

namespace Identity.Application.Identity.Exceptions;

public class AuthentioncationException : AppException
{
    public AuthentioncationException(string error) : base(error)
    {
    }
    public AuthentioncationException(string error, HttpStatusCode httpStatusCode) : base(error, httpStatusCode)
    {
    }
    public AuthentioncationException(string error, Exception exception) : base(exception,error)
    {

    }
}