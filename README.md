# DelegateHandlerPlayground

This playground demonstrates how to use custom `DelegatingHandler` in ASP.NET Core Minimal APIs with [Refit](https://github.com/reactiveui/refit) (.NET 10, C# 14).

## Features
- **Refit-generated clients**: Easily call REST APIs using strongly-typed interfaces.
- **DelegateHandlers**: Add cross-cutting concerns (logging, authentication, etc.) to outgoing HTTP requests.
- **Minimal API structure**: Modern, modular, and easy to extend.

## Key Concepts
### Refit Client
- Define an interface for your API (see `IWeatherApi`).
- Register with `AddRefitClient` in DI.
- Use in endpoints via dependency injection.

### DelegateHandlers
- Create classes inheriting from `DelegatingHandler`.
- Chain multiple handlers for logging, authentication, etc.
- Register with DI and attach to clients using `.AddHttpMessageHandler<T>()`.

## Example Endpoints
- `/weather/{city}`: Calls the WeatherAPI (https://www.weatherapi.com/) to get current weather for the specified city using Refit and two handlers (logging, API key).

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
- [Minimal APIs Tutorial](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-10.0)
- [Refit Documentation](https://github.com/reactiveui/refit)

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---
**.NET 10 / C# 14** | Modern ASP.NET Core | Educational Playground
