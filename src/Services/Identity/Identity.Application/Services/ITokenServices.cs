using Identity.Application.Identity.Dtos;
using Identity.Application.Identity.EntityModel;
using System.Security.Claims;

namespace Identity.Application.Services
{
    public interface ITokenService
    {
        AccessTokenModel GenerateAccessToken(UserModel user);

        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}
