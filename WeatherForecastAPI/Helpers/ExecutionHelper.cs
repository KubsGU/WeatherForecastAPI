﻿using Microsoft.EntityFrameworkCore;
using WeatherForecastAPI.Exceptions;

public static class ExecutionHelper
{
    public static async Task<T> ExecuteWithHandling<T>(
        Func<Task<T>> action,
        ILogger logger,
        string operationDescription)
    {
        try
        {
            return await action();
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, $"Service unavailable during: {operationDescription}");
            throw new ServiceUnavailableException("The external API is currently unavailable. Please try again later.", ex);
        }
        catch (TimeoutException ex)
        {
            logger.LogError(ex, $"Timeout during: {operationDescription}");
            throw new ServiceUnavailableException("The external API request timed out. Please try again later.", ex);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, $"Database error during: {operationDescription}");
            throw new DatabaseOperationException("Database operation failed. Please check the connection and try again.", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Unexpected error during: {operationDescription}");
            throw new Exception("An unexpected error occurred. Please try again later.", ex);
        }
    }
}