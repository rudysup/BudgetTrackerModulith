namespace BudgetTracker.Modules.Expenses.Infrastructure;

public interface IExpensesRepository
{
    Task<IEnumerable<ExpenseDto>> GetAllAsync();
    Task<ExpenseDto> CreateAsync(CreateExpenseDto dto);
}

public record ExpenseDto(Guid Id, string Title, decimal Amount, DateTime CreatedAt);
public record CreateExpenseDto(string Title, decimal Amount);
