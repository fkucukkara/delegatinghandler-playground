# DelegatingHandler Playground

This playground demonstrates how to use custom `DelegatingHandler` in ASP.NET Core Minimal APIs (.NET 10, C# 14). It showcases building a pipeline of HTTP message handlers for cross-cutting concerns like logging, authentication, error handling, and header propagation.

## Main Feature
- **Custom DelegatingHandlers**: Create and chain multiple `DelegatingHandler` classes to intercept and modify outgoing HTTP requests and responses, enabling features like structured logging with ILogger, API key injection, performance tracking, and comprehensive error handling.

## Side Features
- **Refit**: Generate strongly-typed HTTP clients from REST API interfaces for clean, type-safe API calls.
- **Header Propagation Middleware**: Automatically propagate specified headers (e.g., `X-TraceId`) from incoming requests to outgoing HTTP requests using ASP.NET Core's built-in middleware.
- **Health Checks**: Built-in `/health` endpoint for monitoring application status.
- **Comprehensive Error Handling**: Proper exception handling with detailed error responses and logging.
- **Structured Logging**: Uses ASP.NET Core's ILogger for production-ready logging with performance metrics.

## Key Concepts
### Custom DelegatingHandlers
- Inherit from `DelegatingHandler` to create handlers for specific concerns (e.g., structured logging with ILogger, adding authentication, performance tracking).
- Chain handlers in the desired order using `.AddHttpMessageHandler<T>()` on the `HttpClient` builder. Note that built-in handlers like `AddHeaderPropagation()` should be added before custom handlers to ensure proper execution order.
- Handlers execute in the order they are added, allowing for modular and reusable HTTP pipeline logic.
- **Best Practices**:
  - Use dependency injection for `ILogger` and `IConfiguration`.
  - Validate required configuration (e.g., API keys) and throw meaningful exceptions.
  - Track request duration for performance monitoring.
  - Avoid logging sensitive data (e.g., Authorization headers).

### Refit Client
- Define an interface for your API (see `IWeatherApi`).
- Register with `AddRefitClient` in DI for automatic client generation.
- Use in endpoints via dependency injection for seamless API integration.

### Header Propagation Middleware
- Configure which headers to propagate in `Program.cs` using `AddHeaderPropagation`.
- Attach to `HttpClient` instances with `.AddHeaderPropagation()`.
- Use `UseHeaderPropagation()` in the middleware pipeline to enable propagation from incoming requests.

## Example Endpoints
- `/weather/{city}`: Retrieves current weather for a city using Refit, with handlers chained in this order: Header Propagation (adds propagated headers), Logging (logs request details including headers and performance metrics), and API Key (injects and validates the API key as a query parameter). Includes comprehensive error handling with proper HTTP status codes and structured logging.
- `/health`: Health check endpoint for monitoring application status.

## Prerequisites
- Create an account at [WeatherAPI](https://www.weatherapi.com/) to obtain an API key.
- Set the API key in user-secrets: `dotnet user-secrets set "WeatherApi:Key" "your-api-key"`

## How to Run
1. Clone the repo.
2. Set your weather API key using user-secrets as described above.
3. Run the project (`dotnet run`).
4. Explore endpoints with Swagger UI (`/swagger`) or use the health check endpoint (`/health`).

## Troubleshooting
- **Missing API Key Error**: If you see an error about missing API key, ensure you've set it using: `dotnet user-secrets set "WeatherApi:Key" "your-api-key"`
- **404 Not Found**: The city name may be invalid or not found in the WeatherAPI database. Try common city names like "London" or "New York".
- **Health Check**: Visit `/health` to verify the application is running correctly.

## Production-Ready Features
- ✅ Structured logging with ILogger
- ✅ Comprehensive error handling with proper HTTP status codes
- ✅ Request/Response validation
- ✅ Configuration validation (API key required)
- ✅ Performance tracking (request duration)
- ✅ Health checks endpoint
- ✅ Security best practices (no sensitive data in logs)
- ✅ OpenAPI/Swagger documentation

## References
- [ASP.NET Core HTTP Requests](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-10.0)
- [Header Propagation Middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-10.0#header-propagation-middleware)
- [Minimal APIs Tutorial](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-10.0)
- [Refit Documentation](https://github.com/reactiveui/refit)

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---
**.NET 10 / C# 14** | Modern ASP.NET Core | Educational Playground
