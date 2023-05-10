using Dapper;
using MinimalApi.DemoProject.Data;
using MinimalApi.DemoProject.Models;

namespace MinimalApi.DemoProject.Services;

public class PokedexService : IPokedexService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PokedexService(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> CreateAsync(Pokemon pokemon)
    {
        var existingPokemon = await GetById(pokemon.Id);
        if (existingPokemon is not null)
        {
            return false;
        }

        using var connection = await _connectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            @"INSERT INTO Pokemons (Id, Name, Description, Type, HP, Attack, Defense, Speed) 
                    VALUES (@Id, @Name, @Description, @Type, @HP, @Attack, @Defense, @Speed)",
            pokemon);
        return result > 0;
    }

    public async Task<bool> DeleteAsync(int pokemonId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            @"DELETE FROM Pokemons WHERE Id = @Id", new { Id = pokemonId });
        return result > 0;
    }

    public async Task<IEnumerable<Pokemon>> GetAllAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Pokemon>(@"SELECT * FROM Pokemons");
    }

    public async Task<Pokemon?> GetById(int pokemonId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return connection.QuerySingleOrDefault<Pokemon>(
            @"SELECT * FROM Pokemons WHERE Id = @Id", new { Id = pokemonId});
    }

    public async Task<IEnumerable<Pokemon>> SearchByTypeAsync(string type)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QueryAsync<Pokemon>(
            @"SELECT * FROM Pokemons WHERE Type = @Type", new { Type = type });
    }

    public async Task<bool> UpdateAsync(Pokemon pokemon)
    {
        var existingPokemon = await GetById(pokemon.Id);
        if (existingPokemon is null)
        {
            return false;
        }

        using var connection = await _connectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            @"UPDATE Pokemons 
            SET Name = @Name,
                Description = @Description,
                Type = @Type,
                HP = @HP,
                Attack = @Attack,
                Defense = @Defense,
                Speed = @Speed
            WHERE Id = @Id ",
            pokemon);

        return result > 0;
    }
}
