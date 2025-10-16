using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;
using Pokedex.Services;
using Pokedex.Models;

namespace Pokedex.Tests.Services;

public class PokemonServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly PokemonService _service;

    public PokemonServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://pokeapi.co")
        };
        _service = new PokemonService(_httpClient);
    }

    [Fact]
    public async Task GetPokemonAsync_WithValidPokemon_ReturnsCorrectData()
    {
        // Arrange
        var pokemonJson = @"{
            ""name"": ""mewtwo"",
            ""flavor_text_entries"": [
                {
                    ""flavor_text"": ""It was created by a scientist after years of horrific gene splicing and DNA engineering experiments."",
                    ""language"": { ""name"": ""en"" }
                }
            ],
            ""habitat"": { ""name"": ""rare"" },
            ""is_legendary"": true
        }";

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(pokemonJson)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await _service.GetPokemonAsync("mewtwo");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("mewtwo", result.Name);
        Assert.Contains("horrific gene splicing", result.Description);
        Assert.Equal("rare", result.Habitat);
        Assert.True(result.IsLegendary);
    }

    [Fact]
    public async Task GetPokemonAsync_WithInvalidPokemon_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.GetPokemonAsync("invalidpokemon"));
    }

    [Fact]
    public async Task GetPokemonAsync_WithMultipleLanguages_ReturnsEnglishDescription()
    {
        // Arrange
        var pokemonJson = @"{
            ""name"": ""pikachu"",
            ""flavor_text_entries"": [
                {
                    ""flavor_text"": ""Quelque chose en francais"",
                    ""language"": { ""name"": ""fr"" }
                },
                {
                    ""flavor_text"": ""When it is angered, it immediately discharges the energy."",
                    ""language"": { ""name"": ""en"" }
                }
            ],
            ""habitat"": { ""name"": ""forest"" },
            ""is_legendary"": false
        }";

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(pokemonJson)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await _service.GetPokemonAsync("pikachu");

        // Assert
        Assert.Contains("angered", result.Description);
        Assert.DoesNotContain("francais", result.Description);
    }

    [Fact]
    public async Task GetPokemonAsync_CleanupFlavorText_RemoveExtraWhitespace()
    {
        // Arrange
        var pokemonJson = @"{
            ""name"": ""test"",
            ""flavor_text_entries"": [
                {
                    ""flavor_text"": ""Line1\nLine2\n\nLine3"",
                    ""language"": { ""name"": ""en"" }
                }
            ],
            ""habitat"": null,
            ""is_legendary"": false
        }";

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(pokemonJson)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await _service.GetPokemonAsync("test");

        // Assert
        Assert.Equal("Line1 Line2 Line3", result.Description);
        Assert.Equal("unknown", result.Habitat);
    }
}