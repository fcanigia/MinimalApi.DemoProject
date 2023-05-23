using FluentValidation;
using FluentValidation.Results;
using MinimalApi.DemoProject.Endpoints.Internal;
using MinimalApi.DemoProject.Models;
using MinimalApi.DemoProject.Services;

namespace MinimalApi.DemoProject.Endpoints;

public class PokedexEndpoints : IEndpoints
{
    private const string ContentType = "application/json";
    private const string Tag = "Pokedex";
    private const string BaseRoute = "pokedex";

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPokedexService, PokedexService>();
    }

    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(BaseRoute, CreatePokemonAsync)
        .WithName("CreatePokemon")
        .Accepts<Pokemon>(ContentType)
        .Produces<Pokemon>(201)
        .Produces<IEnumerable<ValidationFailure>>(400)
        .WithTags(Tag);

        app.MapGet(BaseRoute, GetAllPokemonsAsync)
        .WithName("GetPokemons")
        .Produces<IEnumerable<Pokemon>>(200)
        .WithTags(Tag);

        app.MapGet($"{BaseRoute}/{{id}}", GetPokemonByIdAsync)
        .WithName("GetPokemon")
        .Produces<Pokemon>(200)
        .Produces(404)
        .WithTags(Tag);

        app.MapPut($"{BaseRoute}/{{id}}", UpdatePokemonAsync)
        .WithName("UpdatePokemon")
        .Accepts<Pokemon>(ContentType)
        .Produces<Pokemon>(200)
        .Produces<IEnumerable<ValidationFailure>>(400)
        .Produces(404)
        .WithTags(Tag);

        app.MapDelete($"{BaseRoute}/{{id}}", DeletePokemonAsync)
        .WithName("DeletePokemon")
        .Produces(204)
        .Produces(404)
        .WithTags(Tag);
    }

    internal static async Task<IResult> CreatePokemonAsync(Pokemon pokemon, 
            IPokedexService pokedexService, IValidator<Pokemon> validator)
    {
        var validationResult = await validator.ValidateAsync(pokemon);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var created = await pokedexService.CreateAsync(pokemon);
        if (!created)
        {
            return Results.BadRequest(new List<ValidationFailure>
                    {
                        new ("Id", "A pokemon with this Id already exists")
                    });
        }

        return Results.Created($"/pokedex/{pokemon.Id}", pokemon);
    }

    internal static async Task<IResult> GetAllPokemonsAsync(IPokedexService pokedexService, 
        string? type)
    {
        if (type is not null && !string.IsNullOrWhiteSpace(type))
        {
            var matchedPokemons = await pokedexService.SearchByTypeAsync(type);
            return Results.Ok(matchedPokemons);
        }

        var pokemons = await pokedexService.GetAllAsync();
        return Results.Ok(pokemons);
    }

    internal static async Task<IResult> GetPokemonByIdAsync(int id, 
        IPokedexService pokedexService)
    {
        var pokemon = await pokedexService.GetById(id);
        return pokemon is not null ? Results.Ok(pokemon) : Results.NotFound();
    }

    internal static async Task<IResult> UpdatePokemonAsync(int id, Pokemon pokemon,
            IPokedexService pokedexService, IValidator<Pokemon> validator)
    {
        pokemon.Id = id;
        var validationResult = await validator.ValidateAsync(pokemon);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var updated = await pokedexService.UpdateAsync(pokemon);
        return updated ? Results.Ok(pokemon) : Results.NotFound();

    }

    internal static async Task<IResult> DeletePokemonAsync(int id, 
        IPokedexService pokedexService)
    {
        var deleted = await pokedexService.DeleteAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}

