using System;
using Moq;
using Xunit;
using Pokedex.Services;

namespace Pokedex.Tests.Services;

public class TranslationStrategyFactoryTests
{
    [Theory]
    [InlineData("cave", false, "yoda")]
    [InlineData("forest", true, "yoda")]
    [InlineData("forest", false, "shakespeare")]
    [InlineData("ocean", false, "shakespeare")]
    [InlineData("CAVE", false, "yoda")] // Case-insensitive test
    public void GetStrategy_ReturnsCorrectTranslationType(
        string habitat,
        bool isLegendary,
        string expectedStrategy)
    {
        // Arrange
        var mockYodaStrategy = new Mock<ITranslationStrategy>();
        mockYodaStrategy.Setup(s => s.TranslationType).Returns("yoda");

        var mockShakespeareStrategy = new Mock<ITranslationStrategy>();
        mockShakespeareStrategy.Setup(s => s.TranslationType).Returns("shakespeare");

        var strategies = new[]
        {
            mockYodaStrategy.Object,
            mockShakespeareStrategy.Object
        };

        var factory = new TranslationStrategyFactory(strategies);

        // Act
        var strategy = factory.GetStrategy(habitat, isLegendary);

        // Assert
        Assert.Equal(expectedStrategy, strategy.TranslationType);
    }
}