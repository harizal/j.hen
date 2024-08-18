using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WApp.Models;
using static WApp.Utlis.Enums;

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

    public class SatuanKerjaViewModel : BaseModel
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

    public class PegawaiViewModel : BaseModel
    {
        public PegawaiType Type { get; set; }
        public string? Nomor { get; set; }
        [Required]
        public string? NIK { get; set; }
        [Required]
        public string? Name { get; set; }

        public JenisKelamin? JenisKelamin { get; set; }
        public string? JenisKelaminText { get; set; }
        public List<SelectListItem>? ListJenisKelamin { get; set; }
        public string? NamaTempatLahir { get; set; }
        public DateTime? TanggalLahir { get; set; }

        public string? StatusPerkawinanID { get; set; }
        public List<SelectListItem>? ListStatusPerkawinan { get; set; }

        public string? PendidikanID { get; set; }
        public List<SelectListItem>? ListPendidikan { get; set; }
        public string ? PendidikanText { get; set; }

        public string? ProdiID { get; set; }
        public List<SelectListItem>? ListProdi { get; set; }
        public string? ProdiText { get; set; }

        public string? SatuanKerjaID { get; set; }
        public List<SelectListItem>? ListSatuanKerja { get; set; }
        public string? SatuanKerjaText { get; set; }

        public string? UnitKerjaID { get; set; }
        public List<SelectListItem>? ListUnitKerja { get; set; }
        public string? UnitKerjaText { get; set; }

        public string? TanggalAwal { get; set; }
        public string? TanggalAkhir { get; set; }

        public string? Keterangan { get; set; }
        public string? StatusID { get; set; }
        
    }
}
