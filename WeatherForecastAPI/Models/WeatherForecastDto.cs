namespace WeatherForecastAPI.Models
{
    public class WeatherForecastDto
    {
        public DateTime ForecastDate { get; set; }
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public int WindDirection { get; set; }
        public bool IsDay { get; set; }
        public int WeatherCode { get; set; }
    }
}
