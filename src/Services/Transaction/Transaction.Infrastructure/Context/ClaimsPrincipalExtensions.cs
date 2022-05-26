
using System.Security.Claims;

namespace Transaction.Infrastructure.Context
{
    internal static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            return int.TryParse(principal.FindFirstValue(BuildingBlocks.TokenHandler.ClaimsIdentity.NameId), out var userId) ? userId : default;
            
        }
        public static string GetFirstName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(BuildingBlocks.TokenHandler.ClaimsIdentity.FirstName);
        }
        public static string GetLastName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(BuildingBlocks.TokenHandler.ClaimsIdentity.LastName);
        }

        public static string GetEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(BuildingBlocks.TokenHandler.ClaimsIdentity.Email);
        }

        public static int GetAccountNumber(this ClaimsPrincipal principal)
        {
            return int.TryParse(principal.FindFirstValue(BuildingBlocks.TokenHandler.ClaimsIdentity.AccountNumber), out var accountNo) ? accountNo : default;
        }

      
    }
}
