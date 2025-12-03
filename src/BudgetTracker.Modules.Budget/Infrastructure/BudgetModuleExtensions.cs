using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Modules.Budget.Infrastructure.Persistence;
using BudgetTracker.Shared;

namespace BudgetTracker.Modules.Budget.Infrastructure;

public static class BudgetModuleExtensions
{
    public static IServiceCollection AddBudgetModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<BudgetMigrationsDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("BudgetDb")));

        var conn = config.GetConnectionString("BudgetDb")
            ?? throw new InvalidOperationException("Missing connection string 'BudgetDb'");

        services.AddSingleton<IDbConnectionFactory>(_ => new DapperDbConnectionFactory(conn));
        services.AddScoped<IBudgetRepository, BudgetRepository>();
        return services;
    }
}
