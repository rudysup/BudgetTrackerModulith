using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BudgetTracker.Shared;

public class DapperDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DapperDbConnectionFactory(IConfiguration config, string connName = "DefaultConnection")
    {
        _connectionString = config.GetConnectionString(connName) 
            ?? throw new ArgumentException($"Connection string '{connName}' not found");
    }

        // NEW CONSTRUCTOR — THIS IS WHAT WE NEED
    public DapperDbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }
    
    public IDbConnection CreateConnection() 
    {
        var conn = new SqlConnection(_connectionString);
        Console.WriteLine($"[DAPPER] Using connection: {conn.Database} on {conn.DataSource}");
        return conn;
    }

    //public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
