namespace BudgetTracker.Modules.Auth.Infrastructure;

public interface IAuthRepository
{
    Task<UserDto?> GetByUsernameAsync(string username);
    Task CreateUserAsync(UserDto user, string passwordHash);
}

public record UserDto(Guid Id, string Username, string PasswordHash, string Role, DateTime CreatedAt);
