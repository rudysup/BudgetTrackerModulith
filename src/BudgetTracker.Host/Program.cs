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

// Register modules
builder.Services.AddAuthModule(config);
builder.Services.AddExpensesModule(config);
builder.Services.AddBudgetModule(config);

var app = builder.Build();

// Middleware
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    try { sp.GetRequiredService<BudgetTracker.Modules.Auth.Infrastructure.Persistence.AuthMigrationsDbContext>().Database.Migrate(); } catch { }
    try { sp.GetRequiredService<BudgetTracker.Modules.Expenses.Infrastructure.Persistence.ExpensesMigrationsDbContext>().Database.Migrate(); } catch { }
    try { sp.GetRequiredService<BudgetTracker.Modules.Budget.Infrastructure.Persistence.BudgetMigrationsDbContext>().Database.Migrate(); } catch { }
}

// ROOT
app.MapGet("/", () => "BudgetTracker API is running!");

// MAP ENDPOINTS — THESE MUST BE HERE
app.MapAuthEndpoints(jwtKey);
app.MapExpensesEndpoints();
app.MapBudgetEndpoints();
app.MapControllers();

app.Run();