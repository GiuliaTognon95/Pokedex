using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics;
using Pokedex.Services;
using Pokedex;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5094");

builder.Services.AddControllers();
builder.Services.AddHttpClient<IPokemonService, PokemonService>();
builder.Services.AddHttpClient<ITranslationService, TranslationService>();
builder.Services.AddScoped<ITranslationStrategy, YodaTranslationStrategy>();
builder.Services.AddScoped<ITranslationStrategy, ShakespeareTranslationStrategy>();
builder.Services.AddScoped<ITranslationStrategyFactory, TranslationStrategyFactory>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Exception handler
builder.Services.AddSingleton<GlobalExceptionHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = feature?.Error ?? new Exception("Unknown error");

        var handler = context.RequestServices.GetService<GlobalExceptionHandler>() 
            ?? new GlobalExceptionHandler();
        await handler.TryHandleAsync(context, exception, CancellationToken.None);
    });
});

app.MapControllers();

Console.WriteLine("🚀 Pokedex API Starting...");
Console.WriteLine("🔌 Listening on: http://0.0.0.0:5094");
Console.WriteLine("📚 Swagger UI: http://localhost:5094/swagger");

app.Run();