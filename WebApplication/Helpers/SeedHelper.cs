using Microsoft.AspNetCore.Identity;
using WApp.Models;
using WApp.Utlis;

namespace WApp.Helpers
{
    public static class SeedHelper
    {
        public static async Task<bool> SeedUserAndRoles(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var existingRole = roleManager.Roles.FirstOrDefault(m => m.Name == Constans.RoleAdministrator);
            if (existingRole == null)
            {
                await roleManager.CreateAsync(new IdentityRole(Constans.RoleAdministrator));
            }

            existingRole = roleManager.Roles.FirstOrDefault(m => m.Name == Constans.RoleUser);
            if (existingRole == null)
            {
                await roleManager.CreateAsync(new IdentityRole(Constans.RoleUser));
            }

            var existingUser = userManager.Users.FirstOrDefault(m => m.Email == Constans.EmailUserDefault);
            if (existingUser == null)
            {
                AppUser user = new AppUser
                {
                    Name = Constans.NameUserDefault,
                    Email = Constans.EmailUserDefault,
                    UserName = Constans.EmailUserDefault
                };

                await userManager.CreateAsync(user, Constans.PasswordUserDefault);
                await userManager.AddToRoleAsync(user, Constans.RoleAdministrator);
                await userManager.AddClaimAsync(user, new System.Security.Claims.Claim("Name", Constans.NameUserDefault));
            }

            return true;
        }
    }
}
