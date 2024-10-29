using Microsoft.Extensions.Logging;
using Moq;

namespace WeatherForecastAPI.Tests.Extensions
{
    public static class LoggerExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> logger, LogLevel level, string message, Times times)
        {
            logger.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                times);
        }
    }
}