

using Application.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class IdentityContext : IIdentityContext
    {
        private IHttpContextAccessor _httpContextAccessor;
        public IdentityContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public ClaimsPrincipal CreateClaimsPrincipal(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var jwtToken = jsonToken as JwtSecurityToken;

            var identity = new ClaimsIdentity(jwtToken.Claims, "Login");
   
            var principal = new ClaimsPrincipal(identity);

            return principal;
        }

        public AuthenticationProperties CreateAuthenticationProperties(string token)
        {
            var accessToken = new AuthenticationToken()
            {
                Name = OpenIdConnectParameterNames.AccessToken,
                Value = token
            };
            AuthenticationToken[] tokens = { accessToken };
            var authenticationProperties = new AuthenticationProperties();
            authenticationProperties.StoreTokens(tokens);
            authenticationProperties.IsPersistent = true;

            return authenticationProperties;
        }

        /// <summary>
        /// This is just to isolate and demonstrate that I can't retrieve the token.
        /// </summary>
        public async Task<string> FetchTokenAsync()
        {
           
            var result = await _httpContextAccessor.HttpContext.AuthenticateAsync();

            if (result.Properties == null)
            {
                throw new Exception("Can't fetch authentication properties within current HttpRequest");
            }

           return result.Properties.GetTokenValue(OpenIdConnectParameterNames.AccessToken);
          
        }

    }
}
