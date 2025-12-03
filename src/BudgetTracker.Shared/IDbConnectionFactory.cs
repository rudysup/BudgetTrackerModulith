using System.Data;

namespace BudgetTracker.Shared;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
