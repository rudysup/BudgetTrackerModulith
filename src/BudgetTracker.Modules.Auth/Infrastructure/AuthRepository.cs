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
        var sql = "SELECT Id, Username, PasswordHash, Role, CreatedAt FROM Users WHERE Username = @Username";
        return await conn.QuerySingleOrDefaultAsync<UserDto>(sql, new { Username = username });
    }

    public async Task CreateUserAsync(UserDto user, string passwordHash)
    {
        using var conn = _factory.CreateConnection();
        var sql = "INSERT INTO Users (Id, Username, PasswordHash, Role, CreatedAt) VALUES (@Id, @Username, @PasswordHash, @Role, @CreatedAt)";
        await conn.ExecuteAsync(sql, new
        {
            user.Id, user.Username, PasswordHash = passwordHash, user.Role, user.CreatedAt
        });
    }
}
