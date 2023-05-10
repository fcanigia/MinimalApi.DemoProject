using FluentValidation;
using MinimalApi.DemoProject.Models;

namespace MinimalApi.DemoProject.Validators;

public class PokemonValidator : AbstractValidator<Pokemon>
{
    public PokemonValidator()
    {
        RuleFor(pokemon => pokemon.Id).InclusiveBetween(1, 151);

        RuleFor(pokemon => pokemon.Name).NotEmpty();
        RuleFor(pokemon => pokemon.Description).NotEmpty();
        RuleFor(pokemon => pokemon.Type).NotEmpty();

        RuleFor(pokemon => pokemon.HP).GreaterThan(1);
        RuleFor(pokemon => pokemon.Attack).GreaterThan(1);
        RuleFor(pokemon => pokemon.Defense).GreaterThan(1);
        RuleFor(pokemon => pokemon.Speed).GreaterThan(1);
    }
}
