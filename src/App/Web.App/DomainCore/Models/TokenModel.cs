
namespace DomainCore.Models
{
    public class TokenModel
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public int ExpiresAt { get; set; }
    }
}
