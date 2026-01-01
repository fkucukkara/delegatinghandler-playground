using Microsoft.AspNetCore.WebUtilities;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add OpenAPI support
builder.Services.AddOpenApi();

// Add health checks
builder.Services.AddHealthChecks();

// Register DelegateHandlers
builder.Services.AddTransient<LoggingHandler>();
builder.Services.AddTransient<ApiKeyHandler>();

// Add header propagation
builder.Services.AddHeaderPropagation(options =>
{
    options.Headers.Add("X-TraceId");
});

// Register Refit client with handlers
builder.Services.AddRefitClient<IWeatherApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.weatherapi.com/v1/"))
    .AddHeaderPropagation()
    .AddHttpMessageHandler<LoggingHandler>()
    .AddHttpMessageHandler<ApiKeyHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseHeaderPropagation();

app.MapHealthChecks("/health");

app.MapGet("/weather/{city}", async (string city, IWeatherApi weatherApi, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Fetching weather for city: {City}", city);

        var result = await weatherApi.GetCurrentWeather(city);

        if (result?.Current is null)
        {
            logger.LogWarning("No weather data returned for city: {City}", city);

            return Results.NotFound(new { error = "Weather data not found for the specified city" });
        }

        logger.LogInformation("Successfully retrieved weather for {City}", city);

        return Results.Ok(result);
    }
    catch (ApiException apiEx)
    {
        logger.LogError(apiEx, "API error while fetching weather for city: {City}", city);

        return Results.Problem(
            detail: apiEx.Message,
            statusCode: (int)apiEx.StatusCode,
            title: "Weather API Error"
        );
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Unexpected error while fetching weather for city: {City}", city);

        return Results.Problem(
            detail: "An unexpected error occurred while fetching weather data",
            statusCode: 500,
            title: "Internal Server Error"
        );
    }
})
.WithName("GetCurrentWeather");

app.Run();

public interface IWeatherApi
{
    [Get("/current.json?q={city}")]
    Task<WeatherResponse> GetCurrentWeather(string city);
}

public record WeatherResponse
{
    public Location? Location { get; set; }
    public Current? Current { get; set; }
}

public record Location
{
    public string? Name { get; set; }
    public string? Country { get; set; }
}

public record Current
{
    public double Temp_C { get; set; }
    public double Temp_F { get; set; }
    public Condition? Condition { get; set; }
    public string? Last_Updated { get; set; }
}

public record Condition
{
    public string? Text { get; set; }
    public string? Icon { get; set; }
}

// DelegateHandler #1: Logging outgoing requests
public class LoggingHandler(ILogger<LoggingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Outgoing Request: {Method} {Uri}", request.Method, request.RequestUri);

        foreach (var header in request.Headers)
        {
            // Avoid logging sensitive headers in production
            if (!header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogInformation("Request Header: {Key} = {Value}", header.Key, string.Join(", ", header.Value));
            }
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var response = await base.SendAsync(request, cancellationToken);
        stopwatch.Stop();

        logger.LogInformation(
            "Response: {StatusCode} {ReasonPhrase} - Duration: {Duration}ms",
            (int)response.StatusCode,
            response.ReasonPhrase,
            stopwatch.ElapsedMilliseconds
        );

        return response;
    }
}

// DelegateHandler #2: Add API Key header
public class ApiKeyHandler(IConfiguration configuration, ILogger<ApiKeyHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Get API key from configuration
        var apiKey = configuration["WeatherApi:Key"];

        if (string.IsNullOrEmpty(apiKey))
        {
            logger.LogError("WeatherApi:Key is not configured. Please set it using user-secrets or environment variables.");
            throw new InvalidOperationException("Weather API key is not configured. Please set the 'WeatherApi:Key' configuration value.");
        }

        // WeatherAPI expects the key as a query parameter
        if (request.RequestUri is not null)
        {
            var uri = QueryHelpers.AddQueryString(request.RequestUri.ToString(), "key", apiKey);
            request.RequestUri = new Uri(uri);

            logger.LogInformation("API key added to request URI");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}