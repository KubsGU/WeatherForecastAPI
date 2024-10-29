using WeatherForecastAPI.Models;

namespace WeatherForecastAPI.Tests.Models
{
    public class WeatherForecastTests
    {
        [Fact]
        public void WeatherForecast_DefaultConstructor_SetsExpectedValues()
        {
            // Act
            var forecast = new WeatherForecast();

            // Assert
            Assert.Equal(0, forecast.Id);
            Assert.Equal(0, forecast.CoordinateId);
            Assert.Equal(default(DateTime), forecast.ForecastDate);
            Assert.Equal(0, forecast.Temperature);
            Assert.Equal(0, forecast.WindSpeed);
            Assert.Equal(0, forecast.WindDirection);
            Assert.False(forecast.IsDay);
            Assert.Equal(0, forecast.WeatherCode);
        }

        [Fact]
        public void WeatherForecast_SetProperties_ReturnsExpectedValues()
        {
            // Arrange
            var forecast = new WeatherForecast
            {
                CoordinateId = 1,
                ForecastDate = new DateTime(2024, 10, 28, 11, 30, 0),
                Temperature = 31.6,
                WindSpeed = 26.4,
                WindDirection = 298,
                IsDay = true,
                WeatherCode = 0
            };

            // Assert
            Assert.Equal(1, forecast.CoordinateId);
            Assert.Equal(new DateTime(2024, 10, 28, 11, 30, 0), forecast.ForecastDate);
            Assert.Equal(31.6, forecast.Temperature);
            Assert.Equal(26.4, forecast.WindSpeed);
            Assert.Equal(298, forecast.WindDirection);
            Assert.True(forecast.IsDay);
            Assert.Equal(0, forecast.WeatherCode);
        }
    }
}
