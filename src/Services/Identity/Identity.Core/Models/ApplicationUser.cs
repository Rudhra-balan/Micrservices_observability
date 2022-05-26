using Microsoft.AspNetCore.Identity;

namespace Identity.Core.Models;

public class ApplicationUser : IdentityUser<int>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}