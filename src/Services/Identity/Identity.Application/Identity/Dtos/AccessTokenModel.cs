using System.ComponentModel.DataAnnotations;

namespace Identity.Application.Identity.Dtos
{
    public record AccessTokenModel 
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }

        public int ExpiresAt { get; set; }

    }
}