using Microsoft.AspNetCore.Mvc;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;


[ApiController]
[Route("api/[controller]")]
public class InfluxWriterController : ControllerBase, IDisposable
{
    private readonly IInfluxWriterServices _influxWriterService;
    private readonly ILogger<InfluxWriterController> _logger;
    private readonly InfluxDBClient _client;

    public InfluxWriterController(IInfluxWriterServices writerService, ILogger<InfluxWriterController> logger, InfluxDBClient influxDbClient)
    {
        _influxWriterService = writerService ?? throw new ArgumentNullException(nameof(writerService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = influxDbClient ?? throw new ArgumentNullException(nameof(influxDbClient));
    }

     [HttpPost]
    public IActionResult PostData([FromBody] List<string> lineProtocolDataList, [FromQuery] string bucket, [FromQuery] string organization, [FromQuery] WritePrecision precision = WritePrecision.S)
    {
        try
        {
            _influxWriterService.WriteData(lineProtocolDataList, bucket, organization, precision);
            return Ok("Data inserted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting data into InfluxDB");
            return StatusCode(500, "Internal Server Error");
        }
    }


   public void Dispose()
    {
        _client?.Dispose();
    }

}