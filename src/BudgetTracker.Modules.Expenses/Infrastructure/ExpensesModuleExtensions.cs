using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.Modules.Expenses.Infrastructure.Persistence;
using BudgetTracker.Shared;

namespace BudgetTracker.Modules.Expenses.Infrastructure;

public static class ExpensesModuleExtensions
{
    public static IServiceCollection AddExpensesModule(this IServiceCollection services, IConfiguration config)
{
    services.AddDbContext<ExpensesMigrationsDbContext>(options =>
        options.UseSqlServer(config.GetConnectionString("ExpensesDb")));

    var conn = config.GetConnectionString("ExpensesDb")
        ?? throw new InvalidOperationException("Missing connection string 'ExpensesDb'");

    services.AddSingleton<IDbConnectionFactory>(_ => new DapperDbConnectionFactory(conn));
    services.AddScoped<IExpensesRepository, ExpensesRepository>();
    return services;
}
}
