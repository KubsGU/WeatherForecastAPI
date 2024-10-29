using WeatherForecastAPI.Models;

namespace WeatherForecastAPI.Tests.Models
{
    public class CoordinateTests
    {
        [Fact]
        public void Coordinate_DefaultConstructor_SetsExpectedValues()
        {
            // Act
            var coordinate = new Coordinate();

            // Assert
            Assert.Equal(0, coordinate.Id);
            Assert.Equal(0, coordinate.Latitude);
            Assert.Equal(0, coordinate.Longitude);
        }

        [Fact]
        public void Coordinate_SetProperties_ReturnsExpectedValues()
        {
            // Arrange
            var coordinate = new Coordinate
            {
                Latitude = 25.0,
                Longitude = 55.125
            };

            // Assert
            Assert.Equal(25.0, coordinate.Latitude);
            Assert.Equal(55.125, coordinate.Longitude);
        }
    }
}
