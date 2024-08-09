using System.ComponentModel.DataAnnotations;

namespace WApp.ViewModels
{
    public class Register
    {
        public required string Name { get; set; }
        public required string Username { get; set; }
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }
        [DataType(DataType.Password)]
        public required string Password { get; set; }
        [Compare("Password", ErrorMessage = "Password don't match")]
        [DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }
    }
}
