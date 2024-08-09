using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WApp.Models;

namespace WApp.ViewModels
{
    public class UserViewModel
    {
        public string? Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password don't match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Role { get; set; }
        public List<SelectListItem>? Roles { get; set; }
    }

    public class PoldaViewModel : BaseModel
    {
        [Required]
        public string Name { get; set; }
    }

    public class PendidikanViewModel : BaseModel
    {
        [Required]
        public string Name { get; set; }
    }

    public class ProdiViewModel : BaseModel
    {
        [Required]
        public string Name { get; set; }
    }

    public class StatusViewModel : BaseModel
    {
        [Required]
        public string Name { get; set; }
    }
}
