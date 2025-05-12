using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace EventHub.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedRolesAndUsers(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Admin role
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Seed test admin user
            var adminUser = await userManager.FindByEmailAsync("admin@eventhub.com");
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = "admin@eventhub.com",
                    Email = "admin@eventhub.com",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
