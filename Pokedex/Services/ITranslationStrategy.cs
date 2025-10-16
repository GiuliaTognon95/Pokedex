namespace Pokedex.Services;

using System.Threading.Tasks;
using Pokedex.Models;

public interface ITranslationStrategy
{
    string TranslationType { get; }
    Task<string> TranslateAsync(string text);
}

public class YodaTranslationStrategy : ITranslationStrategy
{
    private readonly ITranslationService _translationService;

    public string TranslationType => "yoda";

    public YodaTranslationStrategy(ITranslationService translationService)
    {
        _translationService = translationService;
    }

    public Task<string> TranslateAsync(string text) =>
        _translationService.TranslateAsync(text, TranslationType);
}

public class ShakespeareTranslationStrategy : ITranslationStrategy
{
    private readonly ITranslationService _translationService;

    public string TranslationType => "shakespeare";

    public ShakespeareTranslationStrategy(ITranslationService translationService)
    {
        _translationService = translationService;
    }

    public Task<string> TranslateAsync(string text) =>
        _translationService.TranslateAsync(text, TranslationType);
}