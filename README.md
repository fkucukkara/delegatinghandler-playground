# DelegatingHandler Playground

This playground demonstrates how to use custom `DelegatingHandler` in ASP.NET Core Minimal APIs (.NET 10, C# 14). It showcases building a pipeline of HTTP message handlers for cross-cutting concerns like logging, authentication, and header propagation.

## Main Feature
- **Custom DelegatingHandlers**: Create and chain multiple `DelegatingHandler` classes to intercept and modify outgoing HTTP requests and responses, enabling features like logging, API key injection, and more.

## Side Features
- **Refit**: Generate strongly-typed HTTP clients from REST API interfaces for clean, type-safe API calls.
- **Header Propagation Middleware**: Automatically propagate specified headers (e.g., `X-TraceId`) from incoming requests to outgoing HTTP requests using ASP.NET Core's built-in middleware.

## Key Concepts
### Custom DelegatingHandlers
- Inherit from `DelegatingHandler` to create handlers for specific concerns (e.g., logging requests, adding authentication).
- Chain handlers in the desired order using `.AddHttpMessageHandler<T>()` on the `HttpClient` builder. Note that built-in handlers like `AddHeaderPropagation()` should be added before custom handlers to ensure proper execution order.
- Handlers execute in the order they are added, allowing for modular and reusable HTTP pipeline logic.

### Refit Client
- Define an interface for your API (see `IWeatherApi`).
- Register with `AddRefitClient` in DI for automatic client generation.
- Use in endpoints via dependency injection for seamless API integration.

### Header Propagation Middleware
- Configure which headers to propagate in `Program.cs` using `AddHeaderPropagation`.
- Attach to `HttpClient` instances with `.AddHeaderPropagation()`.
- Use `UseHeaderPropagation()` in the middleware pipeline to enable propagation from incoming requests.

## Example Endpoints
- `/weather/{city}`: Retrieves current weather for a city using Refit, with handlers chained in this order: Header Propagation (adds propagated headers), Logging (logs request details including headers), and API Key (injects the API key as a query parameter).

## Prerequisites
- Create an account at [WeatherAPI](https://www.weatherapi.com/) to obtain an API key.
- Set the API key in user-secrets: `dotnet user-secrets set "WeatherApi:Key" "your-api-key"`

## How to Run
1. Clone the repo.
2. Set your weather API key using user-secrets as described above.
3. Run the project (`dotnet run`).
4. Explore endpoints with Swagger UI (`/swagger`).

## References
- [ASP.NET Core HTTP Requests](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-10.0)
- [Header Propagation Middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-10.0#header-propagation-middleware)
- [Minimal APIs Tutorial](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-10.0)
- [Refit Documentation](https://github.com/reactiveui/refit)

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---
**.NET 10 / C# 14** | Modern ASP.NET Core | Educational Playground
