using MinimalApi.DemoProject.Models;

namespace MinimalApi.DemoProject.Services;

public interface IPokedexService
{
    public Task<bool> CreateAsync(Pokemon pokemon);

    public Task<Pokemon?> GetById(int pokemonId);

    public Task<IEnumerable<Pokemon>> GetAllAsync();

    public Task<IEnumerable<Pokemon>> SearchByTypeAsync(string type);

    public Task<bool> UpdateAsync(Pokemon pokemon);

    public Task<bool> DeleteAsync(int pokemonId);
}
