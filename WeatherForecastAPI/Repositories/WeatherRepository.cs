using Microsoft.EntityFrameworkCore;
using WeatherForecastAPI.Data;
using WeatherForecastAPI.Models;

namespace WeatherForecastAPI.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly WeatherDbContext _context;

        public WeatherRepository(WeatherDbContext context)
        {
            _context = context;
        }

        public async Task AddCoordinateAsync(Coordinate coordinate)
        {
            _context.Coordinates.Add(coordinate);
            await _context.SaveChangesAsync();
        }

        public async Task<Coordinate?> GetCoordinateAsync(int id)
        {
            return await _context.Coordinates.FindAsync(id);
        }

        public async Task DeleteCoordinateAsync(int id)
        {
            var coordinate = await GetCoordinateAsync(id);
            if (coordinate != null)
            {
                _context.Coordinates.Remove(coordinate);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Coordinate>> GetAllCoordinatesAsync()
        {
            return await _context.Coordinates.ToListAsync();
        }

        public async Task AddWeatherForecastAsync(WeatherForecast forecast)
        {
            _context.WeatherForecasts.Add(forecast);
            await _context.SaveChangesAsync();
        }

        public async Task<WeatherForecast?> GetLatestWeatherForecastAsync(int coordinateId)
        {
            return await _context.WeatherForecasts
                .Include(f => f.Coordinate)
                .Where(f => f.CoordinateId == coordinateId)
                .OrderByDescending(f => f.ForecastDate)
                .FirstOrDefaultAsync();
        }

        public async Task<Coordinate?> GetCoordinateByLatLongAsync(double latitude, double longitude)
        {
            return await _context.Coordinates
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Latitude == latitude && c.Longitude == longitude);
        }
    }
}
