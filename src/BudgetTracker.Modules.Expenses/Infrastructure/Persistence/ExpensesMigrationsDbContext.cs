using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Modules.Expenses.Infrastructure.Persistence;

public class ExpensesMigrationsDbContext : DbContext
{
    public ExpensesMigrationsDbContext(DbContextOptions<ExpensesMigrationsDbContext> options) : base(options) { }
    public DbSet<ExpenseEntity> Expenses { get; set; } = null!;
}

public class ExpenseEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}
