using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http.Metadata;

namespace BudgetTracker.Modules.Auth.Infrastructure;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app, string jwtKey)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async ([FromServices] IAuthRepository repo, [FromBody] RegisterDto dto) =>
        {
            var existing = await repo.GetByUsernameAsync(dto.Username);
            if (existing != null) return Results.Conflict("Username already exists");

            var id = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;
            var passwordHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(dto.Password));
            var user = new UserDto(id, dto.Username, passwordHash, "User", createdAt);
            await repo.CreateUserAsync(user, passwordHash);
            return Results.Created($"/api/auth/users/{id}", user);
        })
        .AllowAnonymous()
        .Accepts<RegisterDto>("application/json");


        group.MapPost("/login", async ([FromServices] IAuthRepository repo, [FromBody] LoginDto dto) =>
        {
            var user = await repo.GetByUsernameAsync(dto.Username);
            if (user == null) return Results.Unauthorized();

            var sentHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(dto.Password));
            if (sentHash != user.PasswordHash) return Results.Unauthorized();

            var claims = new[] { new Claim(ClaimTypes.Name, user.Username), new Claim(ClaimTypes.Role, user.Role) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer: "BudgetTracker", audience: "BudgetTracker", claims: claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: creds);

            return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        })
        .Accepts<LoginDto>("application/json")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithDisplayName("Login");      // ← THIS ONE LINE IS ENOUGH
    }
}

public record RegisterDto(string Username, string Password);
public record LoginDto(string Username, string Password);
