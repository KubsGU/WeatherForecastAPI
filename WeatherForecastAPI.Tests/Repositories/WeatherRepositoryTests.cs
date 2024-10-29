using Microsoft.EntityFrameworkCore;
using WeatherForecastAPI.Data;
using WeatherForecastAPI.Models;
using WeatherForecastAPI.Repositories;

namespace WeatherForecastAPI.Tests.Repositories
{
    public class WeatherRepositoryTests
    {
        private readonly WeatherDbContext _context;
        private readonly IWeatherRepository _repository;

        public WeatherRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<WeatherDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new WeatherDbContext(options);
            _repository = new WeatherRepository(_context);
        }

        [Fact]
        public async Task AddCoordinateAsync_AddsCoordinateToDatabase()
        {
            // Arrange
            var coordinate = new Coordinate { Latitude = 25.0, Longitude = 55.125 };

            // Act
            await _repository.AddCoordinateAsync(coordinate);
            var savedCoordinate = await _context.Coordinates.FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(savedCoordinate);
            Assert.Equal(coordinate.Latitude, savedCoordinate.Latitude);
            Assert.Equal(coordinate.Longitude, savedCoordinate.Longitude);
        }

        [Fact]
        public async Task GetAllCoordinatesAsync_ReturnsAllCoordinates()
        {
            // Arrange
            _context.Coordinates.AddRange(new List<Coordinate>
            {
                new Coordinate { Latitude = 25.0, Longitude = 55.125 },
                new Coordinate { Latitude = 26.0, Longitude = 56.125 }
            });
            await _context.SaveChangesAsync();

            // Act
            var coordinates = await _repository.GetAllCoordinatesAsync();

            // Assert
            Assert.Equal(2, coordinates.Count);
        }

        [Fact]
        public async Task DeleteCoordinateAsync_DeletesCoordinateFromDatabase()
        {
            // Arrange
            var coordinate = new Coordinate { Latitude = 25.0, Longitude = 55.125 };
            _context.Coordinates.Add(coordinate);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteCoordinateAsync(coordinate.Id);
            var deletedCoordinate = await _context.Coordinates.FindAsync(coordinate.Id);

            // Assert
            Assert.Null(deletedCoordinate);
        }


        [Fact]
        public async Task GetCoordinateByLatLongAsync_ReturnsCoordinate_WhenCoordinateExists()
        {
            // Arrange
            var latitude = 25.0;
            var longitude = 55.125;
            var coordinate = new Coordinate { Latitude = latitude, Longitude = longitude };

            _context.Coordinates.Add(coordinate);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetCoordinateByLatLongAsync(latitude, longitude);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(latitude, result.Latitude);
            Assert.Equal(longitude, result.Longitude);
        }

        [Fact]
        public async Task GetCoordinateByLatLongAsync_ReturnsNull_WhenCoordinateDoesNotExist()
        {
            // Arrange
            var latitude = 40.0;
            var longitude = -74.0;

            // Act
            var result = await _repository.GetCoordinateByLatLongAsync(latitude, longitude);

            // Assert
            Assert.Null(result);
        }
    }
}
