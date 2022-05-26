using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Common
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            return int.TryParse(principal.FindFirstValue("userId"), out var userId) ? userId : default;

        }
        public static string GetFirstName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue("firstName");
        }
        public static string GetLastName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue("LastName");
        }

        public static string GetEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue("email");
        }

        public static int GetAccountNumber(this ClaimsPrincipal principal)
        {
            return int.TryParse(principal.FindFirstValue("acountNumber"), out var accountNo) ? accountNo : default;
        }
        public static string FetchTokenAsync(this IHttpContextAccessor httpContextAccessor)
        {

            var result = httpContextAccessor?.HttpContext.AuthenticateAsync().Result;

            if (result.Properties == null)
            {
                throw new Exception("Can't fetch authentication properties within current HttpRequest");
            }

            return result.Properties.GetTokenValue(OpenIdConnectParameterNames.AccessToken);

        }


    }
}
