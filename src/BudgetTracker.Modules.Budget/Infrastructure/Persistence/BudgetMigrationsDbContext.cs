using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Modules.Budget.Infrastructure.Persistence;

public class BudgetMigrationsDbContext : DbContext
{
    public BudgetMigrationsDbContext(DbContextOptions<BudgetMigrationsDbContext> options) : base(options) { }
    public DbSet<BudgetItemEntity> BudgetItems { get; set; } = null!;
}

public class BudgetItemEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}
