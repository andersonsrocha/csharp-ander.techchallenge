using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using Spendly.Domain.Interfaces;
using TechChallenge.Data.Repositories;
using TechChallenge.Domain.Interfaces;
using TechChallenge.Domain.Interfaces.Repositories;

namespace TechChallenge.Data;

public static class ContextExtension
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.Scan(scan => scan
            .FromAssemblies(typeof(Repository<>).Assembly, typeof(IRepository<>).Assembly)
            .AddClasses(c => c.AssignableTo(typeof(IRepository<>)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }

    public static void AddSqlContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TechChallengeContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("TechDb")));
    }
}