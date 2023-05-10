using FluentValidation.Results;
using FluentValidation;
using MinimalApi.DemoProject.Endpoints.Internal;
using MinimalApi.DemoProject.Models;
using MinimalApi.DemoProject.Services;

namespace MinimalApi.DemoProject.Endpoints
{
    public class PokedexEndpoints : IEndpoints
    {
        public static void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IPokedexService, PokedexService>();
        }

        public static void DefineEndpoints(IEndpointRouteBuilder app)
        {
            app.MapPost("pokedex", async (Pokemon pokemon, IPokedexService pokedexService,
                IValidator<Pokemon> validator) =>
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
                        })
            .WithName("CreatePokemon")
            .Accepts<Pokemon>("application/json")
            .Produces<Pokemon>(201)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags("Pokedex");
        }
    }
}
