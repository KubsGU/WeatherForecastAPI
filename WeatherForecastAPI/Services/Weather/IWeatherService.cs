using WeatherForecastAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WeatherForecastAPI.Services.Weather
{
    public interface IWeatherService
    {
        Task<WeatherResponse?> FetchAndSaveWeatherForecastAsync(AddWeatherRequest request);
        Task<WeatherResponse?> FetchAndSaveWeatherForecastByIdAsync(AddWeatherByIdRequest request);
        Task<List<CoordinateDto>> GetAllCoordinatesAsync();
        Task<WeatherResponse?> GetWeatherForecastAsync(int coordinateId);
        Task<bool> DeleteCoordinateAsync(int id);
    }
}
