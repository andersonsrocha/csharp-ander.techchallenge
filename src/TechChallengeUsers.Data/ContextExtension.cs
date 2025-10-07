using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using TechChallengeUsers.Data.Repositories;
using TechChallengeUsers.Domain.Interfaces;

namespace TechChallengeUsers.Data;

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
        services.AddDbContext<TechChallengeUsersContext>(options => options.UseNpgsql(configuration.GetConnectionString("TechDb")));
    }
}