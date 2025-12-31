using Microsoft.AspNetCore.WebUtilities;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DelegateHandlers
builder.Services.AddTransient<LoggingHandler>();
builder.Services.AddTransient<ApiKeyHandler>();

// Register Refit client with handlers
builder.Services.AddRefitClient<IWeatherApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.weatherapi.com/v1/"))
    .AddHttpMessageHandler<LoggingHandler>()
    .AddHttpMessageHandler<ApiKeyHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/weather/{city}", async (string city, IWeatherApi weatherApi) =>
{
    var result = await weatherApi.GetCurrentWeather(city);
    return Results.Ok(result);
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
public class LoggingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[LoggingHandler] {request.Method} {request.RequestUri}");

        var response = await base.SendAsync(request, cancellationToken);

        Console.WriteLine($"[LoggingHandler] Response: {(int)response.StatusCode} {response.ReasonPhrase}");

        return response;
    }
}

// DelegateHandler #2: Add API Key header
public class ApiKeyHandler(IConfiguration configuration) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Get API key from configuration
        var apiKey = configuration["WeatherApi:Key"];
        if (!string.IsNullOrEmpty(apiKey))
        {
            // WeatherAPI expects the key as a query parameter
            if (request.RequestUri is not null)
            {
                var uri = QueryHelpers.AddQueryString(request.RequestUri.ToString(), "key", apiKey);
                request.RequestUri = new Uri(uri);
            }
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
