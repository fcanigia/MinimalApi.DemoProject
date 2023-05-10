using FluentValidation;
using FluentValidation.Results;
using MinimalApi.DemoProject.Data;
using MinimalApi.DemoProject.Models;
using MinimalApi.DemoProject.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new SqliteConnectionFactory(
        builder.Configuration.GetValue<string>("Database:ConnectionString")!));
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddSingleton<IPokedexService, PokedexService>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

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
});

var databaseInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await databaseInitializer.InitializeAsync();

app.Run();

