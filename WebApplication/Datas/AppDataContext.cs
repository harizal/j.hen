using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WApp.Models;

namespace WApp.Datas
{
    public class AppDataContext(DbContextOptions<AppDataContext> options) : IdentityDbContext<AppUser>(options)
    {
    }
}
