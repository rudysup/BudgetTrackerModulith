using Dapper;
using BudgetTracker.Shared;

namespace BudgetTracker.Modules.Auth.Infrastructure;

public class AuthRepository : IAuthRepository
{
    private readonly IDbConnectionFactory _factory;
    public AuthRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<UserDto?> GetByUsernameAsync(string username)
    {
        using var conn = _factory.CreateConnection();
        // THIS IS THE CORRECT TABLE NAME
        //var sql = "SELECT Id, Username, PasswordHash, Role, CreatedAt FROM [UserEntities] WHERE Username = @Username";
        var sql = "SELECT Id, Username, PasswordHash, Role, CreatedAt FROM dbo.Users WHERE Username = @Username";
        
         return await conn.QuerySingleOrDefaultAsync<UserDto>(sql, new { Username = username });
    }

public async Task CreateUserAsync(UserDto user, string passwordHash)
    {
        using var conn = _factory.CreateConnection();
        // THIS IS THE CORRECT TABLE NAME
        //var sql = "INSERT INTO [UserEntities] (Id, Username, PasswordHash, Role, CreatedAt) VALUES (@Id, @Username, @PasswordHash, @Role, @CreatedAt)";
        var sql = "INSERT INTO dbo.Users (Id, Username, PasswordHash, Role, CreatedAt) VALUES (@Id, @Username, @PasswordHash, @Role, @CreatedAt)";
        
        await conn.ExecuteAsync(sql, new
        {
            user.Id,
            user.Username,
            PasswordHash = passwordHash,
            user.Role,
            user.CreatedAt
        });
    }
}
