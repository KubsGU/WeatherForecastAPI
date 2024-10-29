using System;

namespace WeatherForecastAPI.Models
{
    public class WeatherForecast
    {
        public int Id { get; set; }
        public int CoordinateId { get; set; }
        public DateTime ForecastDate { get; set; } 
        public double Temperature { get; set; } 
        public double WindSpeed { get; set; } 
        public int WindDirection { get; set; } 
        public bool IsDay { get; set; } 
        public int WeatherCode { get; set; } 

        public virtual Coordinate? Coordinate { get; set; }
    }
}
