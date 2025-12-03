using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BudgetTracker.Modules.Auth.Infrastructure.Persistence;

public class AuthMigrationsDbContext : DbContext
{
    public AuthMigrationsDbContext(DbContextOptions<AuthMigrationsDbContext> options) : base(options) { }
    public DbSet<UserEntity> Users { get; set; } = null!;

    public class DesignTimeFactory : IDesignTimeDbContextFactory<AuthMigrationsDbContext>
    {
        public AuthMigrationsDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "BudgetTracker.Host"))
                .AddJsonFile("appsettings.Development.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AuthMigrationsDbContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("AuthDb"));

            return new AuthMigrationsDbContext(optionsBuilder.Options);
        }
    }
}


public class UserEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = "User";
    public DateTime CreatedAt { get; set; }
}
