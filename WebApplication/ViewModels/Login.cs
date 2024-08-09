using System.ComponentModel.DataAnnotations;

namespace WApp.ViewModels
{
    public class Login
    {
        [Required(ErrorMessage = "Username is required.")]
        public required string Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; }
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
