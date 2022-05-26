using System.ComponentModel.DataAnnotations;

namespace DomainCore.Models.Login
{
    public class LoginUserViewModel : BaseViewModel
    {
        #region Public Members

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        #endregion
    }
}