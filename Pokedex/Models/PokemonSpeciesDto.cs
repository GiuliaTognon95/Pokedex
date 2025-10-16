namespace Pokedex.Models;

using System.Text.Json.Serialization;
using System.Collections.Generic;

public class PokemonSpeciesDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("flavor_text_entries")]
    public List<FlavorTextEntry> FlavorTextEntries { get; set; }

    [JsonPropertyName("habitat")]
    public HabitatDto Habitat { get; set; }

    [JsonPropertyName("is_legendary")]
    public bool IsLegendary { get; set; }
}

public class FlavorTextEntry
{
    [JsonPropertyName("flavor_text")]
    public string FlavorText { get; set; }

    [JsonPropertyName("language")]
    public LanguageDto Language { get; set; }
}

public class LanguageDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class HabitatDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}