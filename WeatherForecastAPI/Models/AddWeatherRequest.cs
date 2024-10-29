using System.ComponentModel.DataAnnotations;

namespace WeatherForecastAPI.Models
{
    public class AddWeatherRequest
    {
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double Latitude { get; set; }

        [Range(-100, 100, ErrorMessage = "Longitude must be between -100 and 100.")]
        public double Longitude { get; set; }
    }
}
