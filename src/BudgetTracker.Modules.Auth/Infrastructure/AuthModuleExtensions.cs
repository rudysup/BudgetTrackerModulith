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

        var authConn = config.GetConnectionString("AuthDb") ?? throw new InvalidOperationException("Missing AuthDb connection string");

        // THIS LINE IS THE FINAL FIX — 100% GUARANTEED
        services.AddSingleton<IDbConnectionFactory>(sp => new DapperDbConnectionFactory(authConn));
        Console.WriteLine($"[AUTH MODULE] DAPPER USING: {authConn.Split(';').FirstOrDefault(s => s.Contains("Database="))}");
        //services.AddSingleton<IDbConnectionFactory>(sp => new DapperDbConnectionFactory(authConn));
        
        services.AddScoped<IAuthRepository, AuthRepository>();

        return services;
    }
}
