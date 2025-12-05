using Microsoft.Extensions.DependencyInjection;
using MyServiceCoin.Configuration;
using MyServiceCoin.HealthChecks;
using MyServiceCoin.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<ServiceSetting>(builder.Configuration.GetSection(nameof(ServiceSetting)));
builder.Services.AddScoped<IApiClient,ApiClient>();
builder.Services.AddHealthChecks()
                .AddCheck<CoinsInfoHealthCheck>("CoinsEndpoint");

// -------------------- LOGGING CONFIG --------------------
if (builder.Environment.IsProduction())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddJsonConsole();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
