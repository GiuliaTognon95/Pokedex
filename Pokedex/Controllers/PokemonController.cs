namespace Pokedex.Controllers;

using Microsoft.AspNetCore.Mvc;
using Pokedex.Models;
using Pokedex.Services;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class PokemonController : ControllerBase
{
    private readonly IPokemonService _pokemonService;
    private readonly ITranslationStrategyFactory _strategyFactory;

    public PokemonController(
        IPokemonService pokemonService,
        ITranslationStrategyFactory strategyFactory)
    {
        _pokemonService = pokemonService;
        _strategyFactory = strategyFactory;
    }

    /// <summary>
    /// Get basic Pokemon information
    /// </summary>
    /// <param name="name">The Pokemon name</param>
    /// <returns>Pokemon information</returns>
    [HttpGet("{name}")]
    [ProducesResponseType(typeof(PokemonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PokemonResponse>> GetPokemon(string name)
    {
        var pokemon = await _pokemonService.GetPokemonAsync(name);
        return Ok(pokemon);
    }

    /// <summary>
    /// Get Pokemon information with fun translation
    /// </summary>
    /// <param name="name">The Pokemon name</param>
    /// <returns>Pokemon information with translated description</returns>
    [HttpGet("translated/{name}")]
    [ProducesResponseType(typeof(PokemonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PokemonResponse>> GetTranslatedPokemon(string name)
    {
        var pokemon = await _pokemonService.GetPokemonAsync(name);

        var strategy = _strategyFactory.GetStrategy(pokemon.Habitat, pokemon.IsLegendary);
        pokemon.Description = await strategy.TranslateAsync(pokemon.Description);

        return Ok(pokemon);
    }
}