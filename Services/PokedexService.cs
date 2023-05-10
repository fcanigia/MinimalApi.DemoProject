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
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var result = await connection.ExecuteAsync(
            @"INSERT INTO Pokemons (Id, Name, Description, Type, HP, Attack, Defense, Speed) 
                    VALUES (@Id, @Name, @Description, @Type, @HP, @Attack, @Defense, @Speed)",
            pokemon);
        return result > 0;
    }

    public Task<bool> DeleteAsync(int pokemonId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Pokemon>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Pokemon?> GetById(int pokemonId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Pokemon>> SearchByTypeAsync(string type)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Pokemon pokemon)
    {
        throw new NotImplementedException();
    }
}
