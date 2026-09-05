using Microsoft.AspNetCore.Identity;
using SimpleProject.Data.Models;

namespace SimpleProject.Data.Seed
{
    public class IdentitySeeder
    {
        public static async Task SeedAsync(
       UserManager<AppUser> userManager,
       RoleManager<IdentityRole> roleManager,
       IConfiguration configuration)
        {
            const string adminRole = "Admin";
            const string adminUsername = "admin";
            const string adminEmail = "admin@gmail.com";
            var adminPassword = configuration["Admin:Password"];

            // Create Admin role if it doesn't exist
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            // Check if Admin user exists
            var adminUser = await userManager.FindByNameAsync(adminUsername);

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = adminUsername,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    adminUser,
                    adminPassword
                );

                if (!result.Succeeded)
                {
                    throw new Exception(
                        string.Join(", ", result.Errors.Select(e => e.Description))
                    );
                }
            }

            // Make sure user has Admin role
            if (!await userManager.IsInRoleAsync(adminUser, adminRole))
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}
