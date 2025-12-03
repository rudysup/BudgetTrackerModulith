namespace BudgetTracker.Modules.Budget.Infrastructure;

public interface IBudgetRepository
{
    Task<ForecastDto> GetForecastAsync();
}

public record ForecastDto(decimal TotalIncome, decimal TotalExpenses, decimal ForecastBalance);
