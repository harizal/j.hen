using Microsoft.AspNetCore.Identity;

namespace WApp.Models
{
    public class AppUser : IdentityUser
    {
        public string? Name { get; set; }
    }
}
