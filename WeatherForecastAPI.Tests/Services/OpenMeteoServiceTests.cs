using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using WeatherForecastAPI.Models;
using WeatherForecastAPI.Services.OpenMeteo;
using WeatherForecastAPI.Tests.Extensions;
using Xunit;

public class OpenMeteoServiceTests
{
    private const double Latitude = 25.0;
    private const double Longitude = 55.125;
    private const string ApiUrl = "https://api.open-meteo.com/v1/forecast";
    private static readonly string RequestUrl = $"{ApiUrl}?latitude={Latitude}&longitude={Longitude}&current_weather=true";

    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly Mock<ILogger<OpenMeteoService>> _mockLogger;
    private readonly OpenMeteoService _openMeteoService;

    public OpenMeteoServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object) { BaseAddress = new Uri(ApiUrl) };
        _mockLogger = new Mock<ILogger<OpenMeteoService>>();
        _openMeteoService = new OpenMeteoService(httpClient, _mockLogger.Object);
    }

    [Fact]
    public async Task GetWeatherDataAsync_ReturnsWeatherData_WhenResponseIsValid()
    {
        // Arrange
        var expectedWeatherData = CreateWeatherData();
        SetupHttpResponse(expectedWeatherData, HttpStatusCode.OK);

        // Act
        var result = await _openMeteoService.GetWeatherDataAsync(Latitude, Longitude);

        // Assert
        AssertWeatherData(result, expectedWeatherData);
    }

    [Fact]
    public async Task GetWeatherDataAsync_LogsErrorAndThrows_WhenJsonIsInvalid()
    {
        // Arrange
        var latitude = 25.0;
        var longitude = 55.125;
        var invalidJsonResponse = "{ invalid_json }";
        SetupHttpResponse(invalidJsonResponse, HttpStatusCode.OK);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _openMeteoService.GetWeatherDataAsync(latitude, longitude));
        Assert.IsType<JsonException>(exception.InnerException);
        _mockLogger.VerifyLog(LogLevel.Error, "Failed to parse weather data from Open-Meteo API.",Times.Once());
    }


    [Fact]
    public async Task GetWeatherDataAsync_LogsErrorAndThrows_WhenRequestFails()
    {
        // Arrange
        SetupHttpRequestException();

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _openMeteoService.GetWeatherDataAsync(Latitude, Longitude));
        _mockLogger.VerifyLog(LogLevel.Error, "Network error while contacting Open-Meteo API.",Times.Once());
    }

    #region Helper Methods

    private void SetupHttpResponse(object content, HttpStatusCode statusCode)
    {
        var jsonResponse = content is string str ? str : JsonSerializer.Serialize(content);
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == RequestUrl),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(jsonResponse)
            });
    }

    private void SetupHttpRequestException()
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == RequestUrl),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Request failed"));
    }

    private WeatherData CreateWeatherData() => new WeatherData
    {
        Latitude = Latitude,
        Longitude = Longitude,
        CurrentWeather = new CurrentWeather
        {
            Temperature = 31.6,
            WindSpeed = 26.4,
            WindDirection = 298,
            IsDay = 1,
            WeatherCode = 0
        }
    };

    private void AssertWeatherData(WeatherData? actual, WeatherData expected)
    {
        Assert.NotNull(actual);
        Assert.Equal(expected.Latitude, actual.Latitude);
        Assert.Equal(expected.Longitude, actual.Longitude);
        Assert.Equal(expected.CurrentWeather.Temperature, actual.CurrentWeather.Temperature);
        Assert.Equal(expected.CurrentWeather.WindSpeed, actual.CurrentWeather.WindSpeed);
        Assert.Equal(expected.CurrentWeather.WindDirection, actual.CurrentWeather.WindDirection);
        Assert.Equal(expected.CurrentWeather.IsDay, actual.CurrentWeather.IsDay);
        Assert.Equal(expected.CurrentWeather.WeatherCode, actual.CurrentWeather.WeatherCode);
    }
    #endregion
}
