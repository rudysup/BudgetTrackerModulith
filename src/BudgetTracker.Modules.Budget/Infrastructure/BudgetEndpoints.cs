using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BudgetTracker.Modules.Budget.Infrastructure;

public static class BudgetEndpoints
{
    public static void MapBudgetEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/budget");
        group.MapGet("/forecast", async (IBudgetRepository repo) =>
        {
            var f = await repo.GetForecastAsync();
            return Results.Ok(new { totalIncome = f.TotalIncome, totalExpenses = f.TotalExpenses, forecastBalance = f.ForecastBalance });
        });
    }
}
