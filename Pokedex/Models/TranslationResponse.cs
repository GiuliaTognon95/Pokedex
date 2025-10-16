namespace Pokedex.Models;

using System.Text.Json.Serialization;

public class TranslationResponse
{
    [JsonPropertyName("success")]
    public SuccessDto Success { get; set; }

    [JsonPropertyName("contents")]
    public ContentsDto Contents { get; set; }
}

public class SuccessDto
{
    [JsonPropertyName("total")]
    public int Total { get; set; }
}

public class ContentsDto
{
    [JsonPropertyName("translated")]
    public string Translated { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("translation")]
    public string Translation { get; set; }
}