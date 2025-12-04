using System.Data;
using Microsoft.Data.SqlClient;

namespace BudgetTracker.Shared;

public class DapperDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DapperDbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        Console.WriteLine($"[DAPPER FACTORY] Received connection string for DB: {_connectionString.Split(';').FirstOrDefault(s => s.Trim().StartsWith("Database="))?.Split('=')[1] ?? "UNKNOWN"}");
    }

    public IDbConnection CreateConnection()
    {
        var conn = new SqlConnection(_connectionString);
        conn.Open();
        Console.WriteLine($"[DAPPER] CONNECTED TO DATABASE: {conn.Database}");
        return conn;
    }
}