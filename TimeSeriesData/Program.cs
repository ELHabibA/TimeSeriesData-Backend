using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TimeSeriesData.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("Functions/secret.json")
    .Build();

var settings = config.GetSection("InfluxDB").Get<InfluxDBSettings>();
var influxDBClient = new InfluxDBClient(settings?.ServerUrl, settings?.Token);

builder.Services.AddSingleton(influxDBClient);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IInfluxWriterServices, InfluxWriterServices>();
builder.Services.AddScoped<IInfluxGetServices, InfluxGetServices>();

// Register a hosted service that disposes the InfluxDBClient instance
builder.Services.AddHostedService<InfluxDBClientDisposer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Hosted service that disposes the InfluxDBClient instance
public class InfluxDBClientDisposer : IHostedService
{
    private readonly InfluxDBClient _client;

    public InfluxDBClientDisposer(InfluxDBClient client)
    {
        _client = client;
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _client.Dispose();
        return Task.CompletedTask;
    }
}