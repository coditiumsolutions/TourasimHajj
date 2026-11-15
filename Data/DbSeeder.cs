using Microsoft.AspNetCore.Identity;
using TourismHajj.Helpers;
using TourismHajj.Models;

namespace TourismHajj.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        const string adminEmail = "admin@tourismhajj.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "System Administrator",
                Designation = "Super Admin",
                IsActive = true
            };

            var result = await userManager.CreateAsync(admin, "Admin@12345");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, AppRoles.SuperAdmin);
                await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            }
        }

        const string ownerEmail = "msghauri3@yahoo.com";
        var owner = await userManager.FindByEmailAsync(ownerEmail);
        if (owner == null)
        {
            owner = new ApplicationUser
            {
                UserName = ownerEmail,
                Email = ownerEmail,
                EmailConfirmed = true,
                FullName = "Shahid Ghauri",
                Designation = "Owner",
                IsActive = true
            };

            var ownerResult = await userManager.CreateAsync(owner, "$hahidGhauri786");
            if (ownerResult.Succeeded)
            {
                await userManager.AddToRoleAsync(owner, AppRoles.SuperAdmin);
                await userManager.AddToRoleAsync(owner, AppRoles.Admin);
            }
        }
    }
}
