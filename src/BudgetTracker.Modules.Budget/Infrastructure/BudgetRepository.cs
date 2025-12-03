using Dapper;
using BudgetTracker.Shared;

namespace BudgetTracker.Modules.Budget.Infrastructure;

public class BudgetRepository : IBudgetRepository
{
    private readonly IDbConnectionFactory _factory;
    public BudgetRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<ForecastDto> GetForecastAsync()
    {
        using var conn = _factory.CreateConnection();
        var incomeSql = "SELECT ISNULL(SUM(Amount),0) FROM BudgetItems WHERE Amount > 0";
        var expenseSql = "SELECT ISNULL(SUM(Amount),0) FROM BudgetItems WHERE Amount < 0";
        var totalIncome = await conn.ExecuteScalarAsync<decimal>(incomeSql);
        var totalExpenses = Math.Abs(await conn.ExecuteScalarAsync<decimal>(expenseSql));
        return new ForecastDto(totalIncome, totalExpenses, totalIncome - totalExpenses);
    }
}
