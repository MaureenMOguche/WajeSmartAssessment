using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WajeSmartAssessment.Domain;

namespace WajeSmartAssessment.Infrastructure.SeedData;

/// <summary>
/// Seeds default data for the application
/// </summary>
public class WajeSmartSeedData
{
    /// <summary>
    /// seeds default user
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var appUser = AppUser.CreateAdmin("Super", "Admin", "superadmin", "superadmin@wajesmart.com");

        using (var context = serviceProvider.GetRequiredService<UserManager<AppUser>>())
        {
            if (!await context.Users.AnyAsync(x => x.UserName == appUser.UserName || x.Email == appUser.Email))
            {
                await context.CreateAsync(appUser, "Password@123");
            }

        }

    }
}

#pragma warning disable