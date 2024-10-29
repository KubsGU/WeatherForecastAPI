namespace WeatherForecastAPI.Helpers.DateTimeProvider
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}