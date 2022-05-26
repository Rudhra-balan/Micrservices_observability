
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Services.Interface
{
    public interface IIdentityContext
    {
        ClaimsPrincipal CreateClaimsPrincipal(string token);

        AuthenticationProperties CreateAuthenticationProperties(string token);

        Task<string> FetchTokenAsync();
    }
}
