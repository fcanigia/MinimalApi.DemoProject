using Dapper;

namespace MinimalApi.DemoProject.Data;

public class DatabaseInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DatabaseInitializer(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync(
            @"CREATE TABLE IF NOT EXISTS Pokemons (
                Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                Description TEXT NOT NULL,
                Type TEXT NOT NULL,
                HP INTEGER,
                Attack INTEGER,
                Defense INTEGER,
                Speed INTEGER)"
            );
    }
}