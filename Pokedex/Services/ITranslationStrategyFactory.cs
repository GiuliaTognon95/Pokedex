namespace Pokedex.Services;

using System;
using System.Collections.Generic;
using System.Linq;

public interface ITranslationStrategyFactory
{
    ITranslationStrategy GetStrategy(string habitat, bool isLegendary);
}

public class TranslationStrategyFactory : ITranslationStrategyFactory
{
    private readonly IEnumerable<ITranslationStrategy> _strategies;

    public TranslationStrategyFactory(IEnumerable<ITranslationStrategy> strategies)
    {
        _strategies = strategies;
    }

    public ITranslationStrategy GetStrategy(string habitat, bool isLegendary)
    {
        // Rule: Cave habitat or legendary Pokemon get Yoda translation
        if (isLegendary || habitat?.Equals("cave", StringComparison.OrdinalIgnoreCase) == true)
        {
            return _strategies.First(s => s.TranslationType == "yoda");
        }

        // Default: Shakespeare translation
        return _strategies.First(s => s.TranslationType == "shakespeare");
    }
}