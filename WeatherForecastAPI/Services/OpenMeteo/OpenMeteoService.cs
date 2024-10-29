using System.Text.Json;
using WeatherForecastAPI.Models;

namespace WeatherForecastAPI.Services.OpenMeteo
{
    public class OpenMeteoService : IOpenMeteoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenMeteoService> _logger;

        private const string BaseUrl = "https://api.open-meteo.com/v1/forecast";
        private const string CurrentWeatherQuery = "?latitude={0}&longitude={1}&current_weather=true";

        public OpenMeteoService(HttpClient httpClient, ILogger<OpenMeteoService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<WeatherData?> GetWeatherDataAsync(double latitude, double longitude)
        {
            var apiUrl = BuildApiUrl(latitude, longitude);

            try
            {
                var responseContent = await _httpClient.GetStringAsync(apiUrl);
                return JsonSerializer.Deserialize<WeatherData>(responseContent);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse weather data from Open-Meteo API.");
                throw new InvalidOperationException("Invalid response format from Open-Meteo API", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error while contacting Open-Meteo API.");
                throw new HttpRequestException("Unable to retrieve data from Open-Meteo API", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching weather data from Open-Meteo API.");
                throw;
            }
        }

        private static string BuildApiUrl(double latitude, double longitude)
        {
            return $"{BaseUrl}{string.Format(CurrentWeatherQuery, latitude, longitude)}";
        }
    }
}
