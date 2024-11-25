using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using PhotoSi.AddressService.Core.MapsterConfig;
using PhotoSi.OrderService.Core.MapsterConfig;
using PhotoSi.UserService.Core.MapsterConfig;
using PhotoSiTest.Api.Extensions;
using PhotoSiTest.API.Extensions;
using PhotoSiTest.ProductService.Core.MapsterConfig;


var builder = WebApplication.CreateBuilder(args);

#region Configuration

// Carica appsettings.json
var configuration = new ConfigurationBuilder()
   .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

// Sostituisci il token %BaseDirectory% con il percorso corrente
configuration["Serilog:WriteTo:1:Args:path"] =
    configuration["Serilog:WriteTo:1:Args:path"]!.Replace("%BaseDirectory%", AppContext.BaseDirectory);

#endregion

#region Mapster

AddressMapConfig.RegisterMappings();
ProductMapConfig.RegisterMappings();
UserMapConfig.RegisterMappings();
OrderMapConfig.RegisterMappings();

#endregion

#region Services

builder.Services.AddDbContexts(configuration);
builder.Host.AddLogging(configuration);
builder.Services.AddHealthChecks(configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureServices();

#endregion

var app = builder.Build();

#region Migrations

app.ApplyMigrations();

#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region Middlewares

// Configura i middleware utilizzando il metodo di estensione
app.UseMiddlewares();

#endregion

#region Endpoint for HealthChecks

app.MapHealthChecks("health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });

#endregion


await app.RunAsync();
