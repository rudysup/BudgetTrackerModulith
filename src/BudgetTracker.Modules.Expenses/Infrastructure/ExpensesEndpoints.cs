using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;


namespace BudgetTracker.Modules.Expenses.Infrastructure;

public static class ExpensesEndpoints
{
    public static void MapExpensesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/expenses");

        group.MapGet("/", async (IExpensesRepository repo) => Results.Ok(await repo.GetAllAsync()));
        group.MapPost("/", async (IExpensesRepository repo, CreateExpenseDto dto) =>
        {
            var created = await repo.CreateAsync(dto);
            return Results.Created($"/api/expenses/{created.Id}", created);
        });
    }
}
