using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherForecastAPI.Helpers.DateTimeProvider;
using WeatherForecastAPI.Models;
using WeatherForecastAPI.Repositories;
using WeatherForecastAPI.Services.OpenMeteo;
using WeatherForecastAPI.Services.Weather;
using WeatherForecastAPI.Tests.Extensions;

public class WeatherServiceTests
{
    private readonly Mock<IOpenMeteoService> _mockOpenMeteoService;
    private readonly Mock<IWeatherRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<WeatherService>> _mockLogger;
    private readonly Mock<IDateTimeProvider> _mockDateTimeProvider;
    private readonly WeatherService _weatherService;
    private readonly DateTime _fixedDateTime;

    public WeatherServiceTests()
    {
        _mockOpenMeteoService = new Mock<IOpenMeteoService>();
        _mockRepository = new Mock<IWeatherRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<WeatherService>>();
        _mockDateTimeProvider = new Mock<IDateTimeProvider>();

        _fixedDateTime = new DateTime(2024, 10, 28);
        _mockDateTimeProvider.Setup(d => d.UtcNow).Returns(_fixedDateTime);

        _weatherService = new WeatherService(
            _mockOpenMeteoService.Object,
            _mockRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object,
            _mockDateTimeProvider.Object
        );
    }

    [Fact]
    public async Task FetchAndSaveWeatherForecastAsync_ReturnsResponse_WhenSuccessful()
    {
        // Arrange
        var request = new AddWeatherRequest { Latitude = 25.0, Longitude = 55.125 };
        var weatherData = new WeatherData
        {
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CurrentWeather = new CurrentWeather
            {
                Temperature = 31.6,
                WindSpeed = 26.4,
                WindDirection = 298,
                IsDay = 1,
                WeatherCode = 0
            }
        };

        var coordinate = new Coordinate { Id = 1, Latitude = request.Latitude, Longitude = request.Longitude };
        var forecast = new WeatherForecast
        {
            CoordinateId = coordinate.Id,
            ForecastDate = _mockDateTimeProvider.Object.UtcNow,
            Temperature = weatherData.CurrentWeather.Temperature,
            WindSpeed = weatherData.CurrentWeather.WindSpeed,
            WindDirection = weatherData.CurrentWeather.WindDirection,
            IsDay = weatherData.CurrentWeather.IsDay == 1,
            WeatherCode = weatherData.CurrentWeather.WeatherCode
        };

        // Mock successful response from _openMeteoService
        _mockOpenMeteoService.Setup(s => s.GetWeatherDataAsync(request.Latitude, request.Longitude))
            .ReturnsAsync(weatherData);

        // Mock repository operations
        _mockRepository.Setup(r => r.AddCoordinateAsync(It.IsAny<Coordinate>())).Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.AddWeatherForecastAsync(It.IsAny<WeatherForecast>())).Returns(Task.CompletedTask);

        // Mock mappings
        _mockMapper.Setup(m => m.Map<WeatherResponse>(It.IsAny<Coordinate>()))
            .Returns(new WeatherResponse { CoordinateId = coordinate.Id });
        _mockMapper.Setup(m => m.Map<WeatherForecastDto>(It.IsAny<WeatherForecast>()))
            .Returns(new WeatherForecastDto
            {
                ForecastDate = forecast.ForecastDate,
                Temperature = forecast.Temperature,
                WindSpeed = forecast.WindSpeed,
                WindDirection = forecast.WindDirection,
                IsDay = forecast.IsDay,
                WeatherCode = forecast.WeatherCode
            });

        // Act
        var result = await _weatherService.FetchAndSaveWeatherForecastAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(coordinate.Id, result.CoordinateId);
        Assert.Equal(forecast.ForecastDate, result.Forecast.ForecastDate);
        _mockRepository.Verify(r => r.AddCoordinateAsync(It.IsAny<Coordinate>()), Times.Once);
        _mockRepository.Verify(r => r.AddWeatherForecastAsync(It.IsAny<WeatherForecast>()), Times.Once);
    }

    [Fact]
    public async Task FetchAndSaveWeatherForecastAsync_LogsWarningAndReturnsNull_WhenWeatherDataIsNull()
    {
        // Arrange
        var request = new AddWeatherRequest { Latitude = 25.0, Longitude = 55.125 };
        _mockOpenMeteoService.Setup(s => s.GetWeatherDataAsync(request.Latitude, request.Longitude))
            .ReturnsAsync((WeatherData)null);

        // Act
        var result = await _weatherService.FetchAndSaveWeatherForecastAsync(request);

        // Assert
        Assert.Null(result);
        _mockLogger.VerifyLog(LogLevel.Warning, "Weather data from Open-Meteo API is null or malformed.", Times.Once());
    }

    [Fact]
    public async Task DeleteCoordinateAsync_ReturnsFalseAndLogsWarning_WhenCoordinateNotFound()
    {
        // Arrange
        var coordinateId = 1;
        _mockRepository.Setup(r => r.GetCoordinateAsync(coordinateId))
            .ReturnsAsync((Coordinate)null);

        // Act
        var result = await _weatherService.DeleteCoordinateAsync(coordinateId);

        // Assert
        Assert.False(result);
        _mockLogger.VerifyLog(LogLevel.Warning, $"Coordinate with ID {coordinateId} not found.",Times.Once());
    }

    [Fact]
    public async Task DeleteCoordinateAsync_ReturnsTrue_WhenCoordinateDeletedSuccessfully()
    {
        // Arrange
        var coordinateId = 1;
        _mockRepository.Setup(r => r.GetCoordinateAsync(coordinateId))
            .ReturnsAsync(new Coordinate { Id = coordinateId });
        _mockRepository.Setup(r => r.DeleteCoordinateAsync(coordinateId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _weatherService.DeleteCoordinateAsync(coordinateId);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteCoordinateAsync(coordinateId), Times.Once);
    }

    [Fact]
    public async Task GetAllCoordinatesAsync_ReturnsMappedCoordinates()
    {
        // Arrange
        var coordinates = SetupCoordinateList();
        _mockRepository.Setup(r => r.GetAllCoordinatesAsync()).ReturnsAsync(coordinates);
        _mockMapper.Setup(m => m.Map<List<CoordinateDto>>(coordinates))
            .Returns(SetupCoordinateDtoList(coordinates));

        // Act
        var result = await _weatherService.GetAllCoordinatesAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        _mockRepository.Verify(r => r.GetAllCoordinatesAsync(), Times.Once);
    }

    #region Helper Methods

    private List<Coordinate> SetupCoordinateList() => new List<Coordinate>
    {
        new Coordinate { Id = 1, Latitude = 25.0, Longitude = 55.125 },
        new Coordinate { Id = 2, Latitude = 40.0, Longitude = -74.0 }
    };

    private List<CoordinateDto> SetupCoordinateDtoList(List<Coordinate> coordinates) => new List<CoordinateDto>
    {
        new CoordinateDto { Id = coordinates[0].Id, Latitude = coordinates[0].Latitude, Longitude = coordinates[0].Longitude },
        new CoordinateDto { Id = coordinates[1].Id, Latitude = coordinates[1].Latitude, Longitude = coordinates[1].Longitude }
    };
    #endregion
}
