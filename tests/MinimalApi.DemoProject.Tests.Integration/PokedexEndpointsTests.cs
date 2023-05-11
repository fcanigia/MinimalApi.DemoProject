using Microsoft.AspNetCore.Mvc.Testing;
using MinimalApi.DemoProject.Models;
using Xunit;

namespace MinimalApi.DemoProject.Tests.Integration;

public class PokedexEndpointsTests : 
    IClassFixture<WebApplicationFactory<IApiMarker>>
{
    private readonly WebApplicationFactory<IApiMarker> _factory;

    public PokedexEndpointsTests(WebApplicationFactory<IApiMarker> factory)
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


        // Assert


    }

    private Pokemon GeneratePokemon()
    {
        return new Pokemon
        {
            Id = 7,
            Name = "Charmander",
            Description = "Fire lizard",
            Type = "Fire",
            Attack = 100,
            Defense = 100,
            HP = 100,
            Speed = 10
        };
    }
}
