using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WeatherForecastAPI.Data;
using WeatherForecastAPI.Helpers.DateTimeProvider;
using WeatherForecastAPI.Models;
using WeatherForecastAPI.Repositories;
using WeatherForecastAPI.Services.OpenMeteo;
using WeatherForecastAPI.Services.Weather;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WeatherDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure()));

builder.Services.AddHttpClient();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IOpenMeteoService, OpenMeteoService>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();

builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();
    dbContext.Database.Migrate();
    dbContext.SeedData();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherForecast API V1");
    });
}


app.MapPost("/api/weather", async (IWeatherService weatherService, AddWeatherRequest request) =>
{
    var validationContext = new ValidationContext(request);
    var validationResults = new List<ValidationResult>();

    bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

    if (!isValid)
    {
        return Results.BadRequest(validationResults);
    }
    var forecast = await weatherService.FetchAndSaveWeatherForecastAsync(request);
    return forecast != null ? Results.Created($"/api/weather/{forecast.CoordinateId}", forecast) : Results.BadRequest("Unable to fetch weather data.");
});

app.MapPost("/api/weather/{id}", async (IWeatherService weatherService, AddWeatherByIdRequest request) =>
{
    var forecast = await weatherService.FetchAndSaveWeatherForecastByIdAsync(request);
    return forecast != null ? Results.Created($"/api/weather/{forecast.CoordinateId}", forecast) : Results.BadRequest("Unable to fetch weather data.");
});

app.MapGet("/api/weather/{id}", async (IWeatherService weatherService, int id) =>
{
    var weather = await weatherService.GetWeatherForecastAsync(id);
    return weather != null ? Results.Ok(weather) : Results.NotFound();
});

app.MapGet("/api/weather/coordinates", async (IWeatherService weatherService) =>
{
    var coordinates = await weatherService.GetAllCoordinatesAsync();
    return Results.Ok(coordinates);
});

app.MapDelete("/api/weather/{id}", async (IWeatherService weatherService, int id) =>
{
    var success = await weatherService.DeleteCoordinateAsync(id);
    return success ? Results.NoContent() : Results.NotFound();
});

app.Run();
