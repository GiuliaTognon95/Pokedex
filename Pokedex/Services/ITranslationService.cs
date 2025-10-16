namespace Pokedex.Services;

using System.Threading.Tasks;

public interface ITranslationService
{
    Task<string> TranslateAsync(string text, string translationType);
}