
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Transaction.Application.IContext;

namespace Transaction.Infrastructure.Context
{
    public class IdentityContext : IIdentityContext
    {
        private readonly ClaimsPrincipal _user;

        public IdentityContext(IHttpContextAccessor httpContextAccessor)
        {
            _user = httpContextAccessor.HttpContext?.User;
        }

        public int UserId => _user.GetUserId();

        public string FirstName => _user.GetFirstName();

        public string LastName => _user.GetLastName();

        public string Email => _user.GetEmail();

        public int AccountNumber => _user.GetAccountNumber();
    }
}
