using WeatherForecastAPI.Exceptions;

namespace WeatherForecastAPI.Helpers
{
    public static class ErrorHandler
    {
        public static void HandleException(Exception ex, ILogger logger)
        {
            if (ex is ServiceUnavailableException)
            {
                logger.LogError(ex, "Service unavailable error.");
                throw new ServiceUnavailableException("Service unavailable. Please try again later.", ex);
            }
            if (ex is DatabaseOperationException)
            {
                logger.LogError(ex, "Database operation failed.");
                throw new DatabaseOperationException("Database error occurred. Please try again later.", ex);
            }

            logger.LogError(ex, "An unexpected error occurred.");
            throw new Exception("Unexpected error occurred. Please try again later.", ex);
        }
    }
}