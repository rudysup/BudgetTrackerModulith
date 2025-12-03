using Dapper;
using BudgetTracker.Shared;

namespace BudgetTracker.Modules.Expenses.Infrastructure;

public class ExpensesRepository : IExpensesRepository
{
    private readonly IDbConnectionFactory _factory;
    public ExpensesRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<ExpenseDto>> GetAllAsync()
    {
        using var conn = _factory.CreateConnection();
        var sql = "SELECT Id, Title, Amount, CreatedAt FROM Expenses ORDER BY CreatedAt DESC";
        return await conn.QueryAsync<ExpenseDto>(sql);
    }

    public async Task<ExpenseDto> CreateAsync(CreateExpenseDto dto)
    {
        using var conn = _factory.CreateConnection();
        var e = new ExpenseDto(Guid.NewGuid(), dto.Title, dto.Amount, DateTime.UtcNow);
        var sql = "INSERT INTO Expenses (Id, Title, Amount, CreatedAt) VALUES (@Id, @Title, @Amount, @CreatedAt)";
        await conn.ExecuteAsync(sql, e);
        return e;
    }
}
