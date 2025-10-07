using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TechChallengeUsers.Data.Seeds;

public static class SeedExtension
{
    public static void AddSeeds(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope();
        var provider = scope.ServiceProvider;
        provider.AddRoleSeed();
        provider.AddAdminSeed();
    }
}