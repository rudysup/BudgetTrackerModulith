using Dapper;
using Microsoft.Data.SqlClient;

public class DatabaseInitializer
{
    private readonly IConfiguration _config;

    public DatabaseInitializer(IConfiguration config) => _config = config;

    public async Task InitializeAsync()
    {
        var sql = @"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
            BEGIN
                CREATE TABLE dbo.Users (
                    Id           UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
                    Username     NVARCHAR(256) NOT NULL,
                    PasswordHash NVARCHAR(MAX) NOT NULL,
                    CreatedAt    DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
                    CONSTRAINT UQ_Users_Username UNIQUE (Username)
                );
            END";

        await using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        await conn.OpenAsync();
        await conn.ExecuteAsync(sql);
    }
}