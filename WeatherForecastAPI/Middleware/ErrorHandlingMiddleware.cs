using System.Net;
using WeatherForecastAPI.Exceptions;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ServiceUnavailableException ex)
        {
            _logger.LogError(ex, "Service unavailable error occurred.");
            await HandleExceptionAsync(context, ex, HttpStatusCode.ServiceUnavailable);
        }
        catch (DatabaseOperationException ex)
        {
            _logger.LogError(ex, "Database operation error occurred.");
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode statusCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var errorResponse = new
        {
            message = ex.Message,
            statusCode = context.Response.StatusCode
        };

        return context.Response.WriteAsJsonAsync(errorResponse);
    }
}
