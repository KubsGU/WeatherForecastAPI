using AutoMapper;
using WeatherForecastAPI.Helpers.DateTimeProvider;
using WeatherForecastAPI.Models;
using WeatherForecastAPI.Repositories;
using WeatherForecastAPI.Services.OpenMeteo;
using Microsoft.Extensions.Logging;

namespace WeatherForecastAPI.Services.Weather
{
    public class WeatherService : IWeatherService
    {
        private readonly IOpenMeteoService _openMeteoService;
        private readonly IWeatherRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<WeatherService> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;

        public WeatherService(
            IOpenMeteoService openMeteoService,
            IWeatherRepository repository,
            IMapper mapper,
            ILogger<WeatherService> logger,
            IDateTimeProvider dateTimeProvider)
        {
            _openMeteoService = openMeteoService;
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<bool> DeleteCoordinateAsync(int id)
        {
            return await ExecuteWithLogging(async () =>
            {
                var coordinate = await _repository.GetCoordinateAsync(id);
                if (coordinate == null)
                {
                    _logger.LogWarning($"Coordinate with ID {id} not found.");
                    return false;
                }

                await _repository.DeleteCoordinateAsync(id);
                return true;
            }, $"Delete coordinate with ID {id}");
        }

        public async Task<WeatherResponse?> FetchAndSaveWeatherForecastAsync(AddWeatherRequest request)
        {
            return await ExecuteWithLogging(async () =>
            {
                var coordinate = await GetOrCreateCoordinateAsync(request.Latitude, request.Longitude);
                return await FetchAndSaveWeatherForecastForCoordinate(coordinate);
            }, "Fetch and save weather forecast");
        }

        public async Task<WeatherResponse?> FetchAndSaveWeatherForecastByIdAsync(AddWeatherByIdRequest request)
        {
            return await ExecuteWithLogging(async () =>
            {
                var coordinate = await _repository.GetCoordinateAsync(request.CoordinateId);
                if (coordinate == null)
                {
                    _logger.LogWarning($"Coordinate with ID {request.CoordinateId} not found.");
                    return null;
                }

                return await FetchAndSaveWeatherForecastForCoordinate(coordinate);
            }, "Fetch and save weather forecast by coordinate ID");
        }

        public async Task<List<CoordinateDto>> GetAllCoordinatesAsync()
        {
            return await ExecuteWithLogging(async () =>
            {
                var coordinates = await _repository.GetAllCoordinatesAsync();
                return _mapper.Map<List<CoordinateDto>>(coordinates);
            }, "Retrieve all coordinates");
        }

        public async Task<WeatherResponse?> GetWeatherForecastAsync(int coordinateId)
        {
            return await ExecuteWithLogging(async () =>
            {
                var coordinate = await _repository.GetCoordinateAsync(coordinateId);
                if (coordinate == null)
                {
                    _logger.LogWarning($"Coordinate with ID {coordinateId} not found.");
                    return null;
                }

                var forecast = await _repository.GetLatestWeatherForecastAsync(coordinateId);
                if (forecast == null)
                {
                    _logger.LogWarning($"No weather forecast found for Coordinate ID {coordinateId}.");
                    return null;
                }

                return MapToWeatherResponse(coordinate, forecast);
            }, $"Retrieve weather forecast for Coordinate ID {coordinateId}");
        }

        #region Private Helper Methods

        private async Task<Coordinate> GetOrCreateCoordinateAsync(double latitude, double longitude)
        {
            var coordinate = await _repository.GetCoordinateByLatLongAsync(latitude, longitude);
            if (coordinate == null)
            {
                coordinate = new Coordinate { Latitude = latitude, Longitude = longitude };
                await _repository.AddCoordinateAsync(coordinate);
            }
            return coordinate;
        }

        private async Task<WeatherResponse?> FetchAndSaveWeatherForecastForCoordinate(Coordinate coordinate)
        {
            var weatherData = await _openMeteoService.GetWeatherDataAsync(coordinate.Latitude, coordinate.Longitude);
            if (weatherData?.CurrentWeather == null)
            {
                _logger.LogWarning("Weather data from Open-Meteo API is null or malformed.");
                return null;
            }

            var forecast = new WeatherForecast
            {
                CoordinateId = coordinate.Id,
                ForecastDate = _dateTimeProvider.UtcNow,
                Temperature = weatherData.CurrentWeather.Temperature,
                WindSpeed = weatherData.CurrentWeather.WindSpeed,
                WindDirection = weatherData.CurrentWeather.WindDirection,
                IsDay = weatherData.CurrentWeather.IsDay == 1,
                WeatherCode = weatherData.CurrentWeather.WeatherCode
            };

            await _repository.AddWeatherForecastAsync(forecast);
            return MapToWeatherResponse(coordinate, forecast);
        }

        private WeatherResponse MapToWeatherResponse(Coordinate coordinate, WeatherForecast forecast)
        {
            var response = _mapper.Map<WeatherResponse>(coordinate);
            response.Forecast = _mapper.Map<WeatherForecastDto>(forecast);
            return response;
        }

        private async Task<T> ExecuteWithLogging<T>(Func<Task<T>> action, string operationDescription)
        {
            return await ExecutionHelper.ExecuteWithHandling(action, _logger, operationDescription);
        }

        #endregion
    }
}
