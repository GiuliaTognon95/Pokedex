using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;
using Pokedex.Services;

namespace Pokedex.Tests.Services;

public class TranslationServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly TranslationService _service;

    public TranslationServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://api.funtranslations.com")
        };
        _service = new TranslationService(_httpClient);
    }

    [Fact]
    public async Task TranslateAsync_WithValidTranslation_ReturnsTranslatedText()
    {
        // Arrange
        var translationJson = @"{
            ""success"": { ""total"": 1 },
            ""contents"": {
                ""translated"": ""Created by a scientist after years of horrific gene splicing and DNA engineering experiments, it was."",
                ""text"": ""It was created by a scientist."",
                ""translation"": ""yoda""
            }
        }";

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(translationJson)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await _service.TranslateAsync("It was created by a scientist.", "yoda");

        // Assert
        Assert.NotNull(result);
        Assert.Contains("was", result);
    }

    [Fact]
    public async Task TranslateAsync_WithFailedResponse_ReturnOriginalText()
    {
        // Arrange
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.TooManyRequests
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var originalText = "This is the original text";

        // Act
        var result = await _service.TranslateAsync(originalText, "yoda");

        // Assert
        Assert.Equal(originalText, result);
    }
}