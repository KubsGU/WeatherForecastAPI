namespace WeatherForecastAPI.Models
{
    public class WeatherResponse
    {
        public int CoordinateId { get; set; }
        public CoordinateDto? Coordinate { get; set; }
        public WeatherForecastDto? Forecast { get; set; }
    }
}
