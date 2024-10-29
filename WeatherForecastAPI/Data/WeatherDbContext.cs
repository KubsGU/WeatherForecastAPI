using Microsoft.EntityFrameworkCore;
using WeatherForecastAPI.Models;

namespace WeatherForecastAPI.Data
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }

        public DbSet<Coordinate> Coordinates { get; set; }
        public DbSet<WeatherForecast> WeatherForecasts { get; set; }


        public void SeedData()
        {
            if (!Coordinates.Any())
            {
                Coordinates.AddRange(
                    new Coordinate { Latitude = 25.0, Longitude = 55.125 },
                    new Coordinate { Latitude = 40.7, Longitude = -74.0 }
                );
            }

            if (!WeatherForecasts.Any())
            {
                WeatherForecasts.AddRange(
                    new WeatherForecast
                    {
                        CoordinateId = 1,
                        ForecastDate = DateTime.UtcNow,
                        Temperature = 28.3,
                        WindSpeed = 15.5,
                        WindDirection = 270,
                        IsDay = true,
                        WeatherCode = 0
                    },
                    new WeatherForecast
                    {
                        CoordinateId = 2,
                        ForecastDate = DateTime.UtcNow,
                        Temperature = 15.6,
                        WindSpeed = 10.2,
                        WindDirection = 180,
                        IsDay = false,
                        WeatherCode = 1
                    }
                );
            }

            SaveChanges();
        }
    }
}
