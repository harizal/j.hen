using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WApp.Models;

namespace WApp.Datas
{
    public class AppDataContext(DbContextOptions<AppDataContext> options) : IdentityDbContext<AppUser>(options)
    {
        public DbSet<PoldaModel> Poldas { get; set; }
        public DbSet<PendidikanModel> Pendidikans { get; set; }
        public DbSet<ProdiModel> Prodis { get; set; }
        public DbSet<StatusModel> Statuses { get; set; }
        public DbSet<PegawaiModel> Pegawais { get; set; }
        public DbSet<SatuanKerjaWilayahModel> SatuanKerjas { get; set; }
    }
}
