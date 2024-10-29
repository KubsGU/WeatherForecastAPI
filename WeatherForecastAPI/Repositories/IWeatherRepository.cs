using WeatherForecastAPI.Models;

namespace WeatherForecastAPI.Repositories
{
    public interface IWeatherRepository
    {
        Task AddCoordinateAsync(Coordinate coordinate);
        Task<Coordinate?> GetCoordinateAsync(int id);
        Task DeleteCoordinateAsync(int id);
        Task<List<Coordinate>> GetAllCoordinatesAsync();
        Task AddWeatherForecastAsync(WeatherForecast forecast);
        Task<WeatherForecast?> GetLatestWeatherForecastAsync(int coordinateId);
        Task<Coordinate?> GetCoordinateByLatLongAsync(double latitude, double longitude);

    }
}
