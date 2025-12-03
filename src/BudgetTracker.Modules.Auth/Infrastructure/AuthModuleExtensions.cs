using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Modules.Auth.Infrastructure.Persistence;
using BudgetTracker.Shared;

namespace BudgetTracker.Modules.Auth.Infrastructure;

public static class AuthModuleExtensions
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AuthMigrationsDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("AuthDb")));

        var authConn = config.GetConnectionString("AuthDb")
            ?? throw new InvalidOperationException("Missing connection string 'AuthDb'");

        services.AddSingleton<IDbConnectionFactory>(_ => new DapperDbConnectionFactory(authConn));
        services.AddScoped<IAuthRepository, AuthRepository>();
        return services;
    }
}
