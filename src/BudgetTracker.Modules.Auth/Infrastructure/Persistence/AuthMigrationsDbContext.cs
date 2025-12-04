using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BudgetTracker.Modules.Auth.Infrastructure.Persistence;

// Your existing DbContext and UserEntity stay the same...
public class AuthMigrationsDbContext : DbContext
{
    public AuthMigrationsDbContext(DbContextOptions<AuthMigrationsDbContext> options) : base(options) { }
    public DbSet<UserEntity> Users { get; set; } = null!;
}

public class UserEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = "User";
    public DateTime CreatedAt { get; set; }
}

// Workaround: Design-time factory (add this at the bottom of the file)
public class AuthDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AuthMigrationsDbContext>
{
    public AuthMigrationsDbContext CreateDbContext(string[] args)
    {
        var hostPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "BudgetTracker.Host");
        var config = new ConfigurationBuilder()
            .SetBasePath(hostPath)
            .AddJsonFile("appsettings.Development.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AuthMigrationsDbContext>();
        optionsBuilder.UseSqlServer(config.GetConnectionString("AuthDb"));

        return new AuthMigrationsDbContext(optionsBuilder.Options);
    }
}