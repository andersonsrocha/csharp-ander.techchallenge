using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace TechChallenge.Data.Seeds;

public static class RoleSeed
{
    public static void AddRoleSeed(this IServiceProvider provider)
    {
        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        
        if (!roleManager.RoleExistsAsync("Admin").Result)
            roleManager.CreateAsync(new IdentityRole<Guid>("Admin")).Wait();
        if (!roleManager.RoleExistsAsync("User").Result)
            roleManager.CreateAsync(new IdentityRole<Guid>("User")).Wait();
    }
}