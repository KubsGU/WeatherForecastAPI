namespace WeatherForecastAPI.Helpers.DateTimeProvider
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}