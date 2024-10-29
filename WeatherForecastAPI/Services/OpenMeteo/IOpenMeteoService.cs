using WeatherForecastAPI.Models;

namespace WeatherForecastAPI.Services.OpenMeteo
{
    public interface IOpenMeteoService
    {
        Task<WeatherData?> GetWeatherDataAsync(double latitude, double longitude);
    }
}
