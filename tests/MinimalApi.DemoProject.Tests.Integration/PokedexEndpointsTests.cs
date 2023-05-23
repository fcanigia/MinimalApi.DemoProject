using MinimalApi.DemoProject.Models;

namespace MinimalApi.DemoProject.Tests.Integration;

public class PokedexEndpointsTests : IClassFixture<PokedexApiFactory>, IAsyncLifetime
{
    private readonly PokedexApiFactory _factory;
    private readonly List<int> _createdIds = new();

    public PokedexEndpointsTests(PokedexApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreatePokemon_CreatesPokemon_WhenDataIsCorrect()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var pokemon = GeneratePokemon();

        // Act
        var result = await httpClient.PostAsJsonAsync("/pokedex", pokemon);
        _createdIds.Add(pokemon.Id);
        var createdPokemon = await result.Content.ReadFromJsonAsync<Pokemon>();

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        createdPokemon.Should().BeEquivalentTo(pokemon);
        result.Headers.Location.Should().Be($"/pokedex/{pokemon.Id}");
    }

    [Fact]
    public async Task CreatePokemon_Fails_WhenIdIsInvalid()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var pokemon = GeneratePokemon();
        pokemon.Id = 170;

        // Act
        var result = await httpClient.PostAsJsonAsync("/pokedex", pokemon);
        _createdIds.Add(pokemon.Id);
        var errors = await result.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var error = errors!.Single();

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Id");
    }

    [Fact]
    public async Task CreatePokemon_Fails_WhenPokemonExists()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var pokemon = GeneratePokemon();

        // Act
        await httpClient.PostAsJsonAsync("/pokedex", pokemon);
        _createdIds.Add(pokemon.Id);
        var result = await httpClient.PostAsJsonAsync("/pokedex", pokemon);
        var errors = await result.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var error = errors!.Single();

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Id");
        error.ErrorMessage.Should().Be("A pokemon with this Id already exists");
    }

    [Fact]
    public async Task GetPokemon_ReturnsPokemon_WhenPokemonExists()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var pokemon = GeneratePokemon();
        await httpClient.PostAsJsonAsync("/pokedex", pokemon);
        _createdIds.Add(pokemon.Id);

        // Act
        var result = await httpClient.GetAsync($"/pokedex/{pokemon.Id}");
        var existingPokemon = await result.Content.ReadFromJsonAsync<Pokemon>();

        // Assert
        existingPokemon.Should().BeEquivalentTo(pokemon);
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPokemon_ReturnsNotFound_WhenPokemonDoesNotExists()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var pokemon = GeneratePokemon();

        // Act
        var result = await httpClient.GetAsync($"/pokedex/{pokemon.Id}");

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllPokemons_ReturnsAllPokemons_WhenPokemonExists()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var pokemon = GeneratePokemon();
        await httpClient.PostAsJsonAsync("/pokedex", pokemon);
        _createdIds.Add(pokemon.Id);
        var pokemons = new List<Pokemon> { pokemon };

        // Act
        var result = await httpClient.GetAsync("/pokedex");
        var returnedPokemons = await result.Content.ReadFromJsonAsync<List<Pokemon>>();

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        returnedPokemons.Should().BeEquivalentTo(pokemons);
    }

    [Fact]
    public async Task GetAllPokemons_ReturnsNoPokemons_WhenNoPokemonExists()
    {
        // Arrange
        var httpClient = _factory.CreateClient();

        // Act
        var result = await httpClient.GetAsync("/pokedex");
        var returnedPokemons = await result.Content.ReadFromJsonAsync<List<Pokemon>>();

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        returnedPokemons.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchPokemons_ReturnsPokemons_WhenTypeMatches()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var pokemon = GeneratePokemon();
        await httpClient.PostAsJsonAsync("/pokedex", pokemon);
        _createdIds.Add(pokemon.Id);
        var pokemons = new List<Pokemon> { pokemon };

        // Act
        var result = await httpClient.GetAsync($"/pokedex?type={pokemon.Type}");
        var returnedPokemons = await result.Content.ReadFromJsonAsync<List<Pokemon>>();

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        returnedPokemons.Should().BeEquivalentTo(pokemons);
    }

    [Fact]
    public async Task UpdatePokemon_UpdatesPokemon_WhenDataIsCorrect()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var pokemon = GeneratePokemon();
        await httpClient.PostAsJsonAsync("/pokedex", pokemon);
        _createdIds.Add(pokemon.Id);

        // Act
        pokemon.Name = "Charmeleon";
        var result = await httpClient.PutAsJsonAsync($"/pokedex/{pokemon.Id}", pokemon);
        var updatedPokemon = await result.Content.ReadFromJsonAsync<Pokemon>();

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        updatedPokemon.Should().BeEquivalentTo(pokemon);
    }

    [Fact]
    public async Task UpdatePokemon_DoesNotUpdatesPokemon_WhenDataIsIncorrect()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var pokemon = GeneratePokemon();
        await httpClient.PostAsJsonAsync("/pokedex", pokemon);
        _createdIds.Add(pokemon.Id);

        // Act
        pokemon.Name = string.Empty;
        var result = await httpClient.PutAsJsonAsync($"/pokedex/{pokemon.Id}", pokemon);
        var errors = await result.Content.ReadFromJsonAsync<IEnumerable<ValidationError>>();
        var error = errors!.Single();

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        error.PropertyName.Should().Be("Name");
        error.ErrorMessage.Should().Be("'Name' must not be empty.");
    }

    [Fact]
    public async Task UpdatePokemon_ReturnsNotFound_WhenPokemonDoesNotExist()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var pokemon = GeneratePokemon();

        // Act
        var result = await httpClient.PutAsJsonAsync($"/pokedex/{pokemon.Id}", pokemon);

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePokemon_ReturnsNotFound_WhenPokemonDoesNotExists()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var pokemon = GeneratePokemon();

        // Act
        var result = await httpClient.DeleteAsync($"/pokedex/{pokemon.Id}");

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePokemon_ReturnsNoContent_WhenPokemonExists()
    {
        // Arrange
        var httpClient = _factory.CreateClient();
        var pokemon = GeneratePokemon();
        await httpClient.PostAsJsonAsync("/pokedex", pokemon);
        _createdIds.Add(pokemon.Id);

        // Act
        var result = await httpClient.DeleteAsync($"/pokedex/{pokemon.Id}");

        // Assert
        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }


    private Pokemon GeneratePokemon(string name = "Charmander")
    {
        return new Pokemon
        {
            Id = GenerateId(),
            Name = name,
            Description = "Fire lizard",
            Type = "Fire",
            Attack = GenerateStat(),
            Defense = GenerateStat(),
            HP = GenerateStat(),
            Speed = GenerateStat()
        };
    }

    private int GenerateId()
    {
        return Random.Shared.Next(1, 151);
    }

    private int GenerateStat()
    {
        return Random.Shared.Next(1, 100);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        var httpClient = _factory.CreateClient();
        foreach (var createdId in _createdIds)
        {
            await httpClient.DeleteAsync($"/pokedex/{createdId}");
        }
    }
}
