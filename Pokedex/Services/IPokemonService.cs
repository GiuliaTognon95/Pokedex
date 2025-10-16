namespace Pokedex.Services;

using Pokedex.Models;
using System.Threading.Tasks;

public interface IPokemonService
{
    Task<PokemonResponse> GetPokemonAsync(string name);
}