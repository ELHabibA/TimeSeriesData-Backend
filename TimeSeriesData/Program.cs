using InfluxDB.Client;



var builder = WebApplication.CreateBuilder(args);


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

// Our Services
builder.Services.AddScoped<IInfluxWriterService, InfluxWriterService>();
builder.Services.AddScoped<IInfluxFetcherService, InfluxFetcherService>();

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

