using System.Data;

namespace MinimalApi.DemoProject.Data;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
}
