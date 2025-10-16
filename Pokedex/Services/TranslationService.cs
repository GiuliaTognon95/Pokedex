namespace Pokedex.Services;

using System.Text.Json;
using Pokedex.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

public class TranslationService : ITranslationService
{
    private readonly HttpClient _httpClient;
    private const string FunTranslationsBaseUrl = "https://api.funtranslations.com/translate";

    public TranslationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> TranslateAsync(string text, string translationType)
    {
        try
        {
            var url = $"{FunTranslationsBaseUrl}/{translationType}";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("text", text)
            });

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return text;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var translationResponse = JsonSerializer.Deserialize<TranslationResponse>(responseContent);

            return translationResponse?.Contents?.Translated ?? text;
        }
        catch
        {
            return text;
        }
    }
}