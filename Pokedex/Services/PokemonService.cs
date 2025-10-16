namespace Pokedex.Services;

using System.Text.Json;
using Pokedex.Models;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class PokemonService : IPokemonService
{
    private readonly HttpClient _httpClient;
    private const string PokeApiBaseUrl = "https://pokeapi.co/api/v2";

    public PokemonService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PokemonResponse> GetPokemonAsync(string name)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{PokeApiBaseUrl}/pokemon-species/{name.ToLower()}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var species = JsonSerializer.Deserialize<PokemonSpeciesDto>(content);

            if (species == null)
                throw new InvalidOperationException("Failed to deserialize Pokemon species data");

            return new PokemonResponse
            {
                Name = species.Name,
                Description = GetEnglishDescription(species.FlavorTextEntries),
                Habitat = species.Habitat?.Name ?? "unknown",
                IsLegendary = species.IsLegendary
            };
        }
        catch (HttpRequestException)
        {
            throw new InvalidOperationException($"Pokemon '{name}' not found");
        }
    }

    private static string GetEnglishDescription(List<FlavorTextEntry> entries)
    {
        var englishEntry = entries
            .FirstOrDefault(e => e.Language?.Name == "en");

        if (englishEntry == null)
            throw new InvalidOperationException("No English description found");

        return Regex.Replace(
            englishEntry.FlavorText,
            @"\s+",
            " "
        ).Trim();
    }
}