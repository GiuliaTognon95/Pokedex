# Pokedex API

A fun REST API that returns Pokemon information with optional playful translations using Yoda or Shakespeare speak.

## Features

- **Basic Pokemon Information**: Fetch standard Pokemon data from PokéAPI
- **Translated Descriptions**: Get Pokemon descriptions translated to Yoda or Shakespeare speak
- **Smart Translation Logic**: Automatically selects translation type based on Pokemon attributes
- **Error Handling**: Graceful fallbacks when external APIs are unavailable
- **Comprehensive Testing**: Unit tests with high-value assertions
- **Docker Support**: Ready to containerize

## Quick Start

### Prerequisites

- .NET 8 SDK or later
- Docker (optional, for containerized deployment)

### Running Locally

```bash
# Clone the repository
git clone <repository-url>
cd Pokedex

# Restore dependencies
dotnet restore

# Run the application
dotnet run --project Pokedex

# The API will be available at http://localhost:5094
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

### Using Docker

```bash
# Build the Docker image
docker build -t pokedex-api .

# Run the container
docker run -p 5094:8080 pokedex-api
```

## API Endpoints

### 1. Get Basic Pokemon Information

```
GET /pokemon/{name}
```

**Example:**
```bash
curl http://localhost:5000/pokemon/mewtwo
```

**Response:**
```json
{
  "name": "mewtwo",
  "description": "It was created by a scientist after years of horrific gene splicing and DNA engineering experiments.",
  "habitat": "rare",
  "isLegendary": true
}
```

### 2. Get Pokemon with Translated Description

```
GET /pokemon/translated/{name}
```

**Example:**
```bash
curl http://localhost:5000/pokemon/translated/mewtwo
```

**Response:**
```json
{
  "name": "mewtwo",
  "description": "Created by a scientist after years of horrific gene splicing and DNA engineering experiments, it was.",
  "habitat": "rare",
  "isLegendary": true
}
```

**Translation Logic:**
- **Yoda Translation**: Applied when Pokemon is legendary OR habitat is "cave"
- **Shakespeare Translation**: Applied to all other Pokemon
- **Fallback**: If translation fails, the original description is returned

## Architecture & Design Decisions

### Dependency Injection

All services are registered in the dependency injection container for:
- Easy testing (services can be mocked)
- Loose coupling between components
- Clean separation of concerns

### Strategy Pattern for Translations

The `ITranslationStrategy` interface and `TranslationStrategyFactory` allow:
- Easy addition of new translation types without modifying existing code
- Clear decision logic for which strategy to apply
- Flexibility for future enhancements

**Production Consideration**: This pattern makes it simple to add new strategies (e.g., pirate speak) without touching the controller or factory logic.

### Service Layer Architecture

- **PokemonService**: Handles all PokéAPI interactions and data mapping
- **TranslationService**: Manages FunTranslations API calls with error handling
- **Controllers**: Minimal logic, only orchestrate services

**Benefit**: Business logic is separate from HTTP concerns, making it testable and reusable.

### Global Exception Handling

A middleware layer catches all exceptions and returns standardized error responses:
- 404 Not Found: When Pokemon doesn't exist
- 500 Internal Server Error: For unexpected errors

**Production Consideration**: In production, you'd want structured logging integrated here.

### Graceful Degradation

- If translation API is unavailable (rate limited or down), the original description is returned
- If habitat data is missing, it defaults to "unknown"
- The API remains functional even if external dependencies fail

## Production Considerations

### 1. **Rate Limiting**

The FunTranslations API has strict rate limits (120 requests/hour for free tier). In production:

```csharp
// Implement exponential backoff and retry logic
services.AddHttpClient<ITranslationService, TranslationService>()
    .AddTransientHttpErrorPolicy()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: retryAttempt => 
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        handledFailurePolicy: response => 
            response.StatusCode == System.Net.HttpStatusCode.TooManyRequests
    );
```

### 2. **Caching**

Add Redis caching for:
- Pokemon data (rarely changes)
- Translation results (if same text is requested again)

```csharp
services.AddStackExchangeRedisCache(options => 
    options.Configuration = builder.Configuration.GetConnectionString("Redis"));
```

### 3. **Logging & Monitoring**

Add structured logging to track:
- API calls and response times
- External API failures
- Translation service performance

```csharp
services.AddSerilog(config => 
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/app.log"));
```

### 4. **API Documentation**

Enable Swagger/OpenAPI documentation for consumers. This is already configured in the template above.

### 5. **Input Validation**

Add validation for Pokemon name input:
- Max length checks
- Character validation
- Rate limiting per client

### 6. **Authentication & Authorization**

For production APIs, consider:
- API key authentication
- JWT tokens for user-specific features
- Rate limiting per user/key

### 7. **Circuit Breaker Pattern**

Protect against cascading failures when external APIs go down:

```csharp
services.AddHttpClient<IPokemonService, PokemonService>()
    .AddTransientHttpErrorPolicy()
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5,
        durationOfBreak: TimeSpan.FromSeconds(30)
    );
```

### 8. **Database Integration**

Consider adding a database layer to cache Pokemon data and avoid repeated external API calls:
- Entity Framework Core with SQL Server or PostgreSQL
- Cache warming on startup
- Periodic refresh of data

### 9. **Tests in Production**

Consider implementing:
- Integration tests that run against staging environment
- Contract testing against external APIs
- Load testing to verify performance under stress

## Project Structure

```
Pokedex/
├── Controllers/
│   └── PokemonController.cs
├── Models/
│   ├── PokemonResponse.cs
│   ├── PokemonSpeciesDto.cs
│   └── TranslationResponse.cs
├── Services/
│   ├── IPokemonService.cs
│   ├── PokemonService.cs
│   ├── ITranslationService.cs
│   ├── TranslationService.cs
│   ├── ITranslationStrategy.cs
│   └── ITranslationStrategyFactory.cs
├── Middleware/
│   └── GlobalExceptionHandler.cs
├── Program.cs
├── Pokedex.csproj
└── Dockerfile
```

## Key Testing Strategies

### Unit Tests Included

1. **PokemonService Tests**
   - Valid Pokemon retrieval
   - Invalid Pokemon error handling
   - Multi-language flavor text filtering
   - Text cleanup (whitespace normalization)

2. **TranslationStrategyFactory Tests**
   - Correct strategy selection based on habitat and legendary status
   - Case-insensitive habitat matching

3. **TranslationService Tests**
   - Successful translation
   - Graceful fallback on API failure
   - Rate limit handling

**What We Test**: Behavior and edge cases rather than implementation details. Tests use Moq to isolate components.

## External APIs

- **PokéAPI**: https://pokeapi.co/
- **FunTranslations API**: https://funtranslations.com/

## Error Responses

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Sorry! I'm not able to find it :(",
  "status": 404,
  "detail": "Pokemon 'notapokemon' not found"
}
```

## Future Enhancements

- Add caching layer (Redis)
- Implement structured logging (OpenSearch)
- Add API versioning
- Support multiple languages beyond English
- Implement pagination for bulk requests
- Ad
