using InstallmentCRM.Persistence.Identity;
using InstallmentCRM.Shared.Constants;
using Microsoft.AspNetCore.Identity;

namespace InstallmentCRM.API.Extensions;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(IServiceProvider services)
    {
        var roleManager =
            services.GetRequiredService<RoleManager<ApplicationRole>>();

        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new ApplicationRole
                    {
                        Name = role
                    });
            }
        }
    }
}
