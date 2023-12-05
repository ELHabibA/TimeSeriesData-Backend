using Microsoft.AspNetCore.Mvc;
using InfluxDB.Client;


[ApiController]
[Route("api/[controller]")]
public class InfluxWriterController : ControllerBase, IDisposable
{
    private readonly IInfluxWriterService _influxWriterService;
    private readonly ILogger<InfluxWriterController> _logger;
    private readonly InfluxDBClient _client;

    public InfluxWriterController(IInfluxWriterService writerService, ILogger<InfluxWriterController> logger, InfluxDBClient influxDbClient)
    {
        _influxWriterService = writerService ?? throw new ArgumentNullException(nameof(writerService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = influxDbClient ?? throw new ArgumentNullException(nameof(influxDbClient));
    }

     [HttpPost]
    public IActionResult PostData([FromBody] List<string> lineProtocolDataList, [FromQuery] string bucket, [FromQuery] string organization, [FromQuery] string precision = "s")
    {
        _logger.LogInformation("Received request to insert data. Bucket: {Bucket}, Organization: {Organization}, Precision: {Precision}, Number of records: {Count}",
            bucket, organization, precision, lineProtocolDataList.Count);

        try
        {
            _influxWriterService.WriteData(lineProtocolDataList, bucket, organization, precision);

            _logger.LogInformation("Data insertion successful.");

            return Ok("Data inserted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting data into InfluxDB. Bucket: {Bucket}, Organization: {Organization}, Precision: {Precision}",
                bucket, organization, precision);
            return StatusCode(500, "Internal Server Error");
        }
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}