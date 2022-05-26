using System.Net;

namespace Identity.Application.Identity.Exceptions;

public class RegisterIdentityUserException : AppException
{
    public RegisterIdentityUserException(string error) : base(error)
    {
    }
    public RegisterIdentityUserException(string error, HttpStatusCode httpStatusCode) : base(error, httpStatusCode)
    {
    }
    public RegisterIdentityUserException(string error, Exception exception) : base(exception, error)
    {

    }
}