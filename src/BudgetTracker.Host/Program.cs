using BudgetTracker.Modules.Auth.Infrastructure;
using BudgetTracker.Modules.Budget.Infrastructure;
using BudgetTracker.Modules.Expenses.Infrastructure;
using BudgetTracker.Shared;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var jwtKey = config["Jwt:Key"] ?? "SuperSecretKey12345678901234567890";

// Core services
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", p => p
       .WithOrigins("http://localhost:4200")
       .AllowAnyHeader()
       .AllowAnyMethod()
       .AllowCredentials());
});

// JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "BudgetTracker",
            ValidAudience = "BudgetTracker",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// REGISTER MODULES — ONLY ONCE, HERE
//builder.Services.AddAuthModule(config);

builder.Services.AddExpensesModule(builder.Configuration);
builder.Services.AddBudgetModule(builder.Configuration);
builder.Services.AddAuthModule(builder.Configuration);   // ← AUTH MUST BE LAST

var app = builder.Build();

// Middleware
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();

// APPLY MIGRATIONS — THIS WORKS BECAUSE MODULES ARE ALREADY REGISTERED ABOVE
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;

    try { sp.GetRequiredService<BudgetTracker.Modules.Auth.Infrastructure.Persistence.AuthMigrationsDbContext>().Database.Migrate(); } catch (Exception ex) { Console.WriteLine($"Auth error: {ex.Message}"); }
    try { sp.GetRequiredService<BudgetTracker.Modules.Expenses.Infrastructure.Persistence.ExpensesMigrationsDbContext>().Database.Migrate(); } catch (Exception ex) { Console.WriteLine($"Expenses error: {ex.Message}"); }
    try { sp.GetRequiredService<BudgetTracker.Modules.Budget.Infrastructure.Persistence.BudgetMigrationsDbContext>().Database.Migrate(); } catch (Exception ex) { Console.WriteLine($"Budget error: {ex.Message}"); }
}

// ROOT
app.MapGet("/", () => "BudgetTracker API is running!");

// MAP ENDPOINTS
app.MapAuthEndpoints(jwtKey);
Console.WriteLine("AUTH ENDPOINTS MAPPED — /api/auth/register is ready");
app.MapExpensesEndpoints();
app.MapBudgetEndpoints();
app.MapControllers();

app.Run();