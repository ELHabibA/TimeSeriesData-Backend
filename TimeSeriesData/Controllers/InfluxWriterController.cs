using Microsoft.AspNetCore.Mvc;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using TimeSeriesData.Services;


[ApiController]
[Route("api/[controller]")]
public class InfluxWriterController : ControllerBase, IDisposable
{
    private readonly IInfluxWriterServices _influxWriterService;
    private readonly IInfluxGetServices _influxGetService;
    private readonly ILogger<InfluxWriterController> _logger;
    private readonly InfluxDBClient _client;

    public InfluxWriterController(IInfluxWriterServices writerService, IInfluxGetServices getService, ILogger<InfluxWriterController> logger, InfluxDBClient influxDbClient)
    {
        _influxWriterService = writerService ?? throw new ArgumentNullException(nameof(writerService));
        _influxGetService = getService ?? throw new ArgumentNullException(nameof(getService)); 
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

    [HttpGet]
    public IActionResult GetData([FromQuery] string bucket, [FromQuery] string organization, [FromQuery] DateTime? startTime = null, [FromQuery] DateTime? endTime = null)
    {
        _logger.LogInformation("GetData started. Bucket: {Bucket}, Organization: {Organization}", bucket, organization);

        try
        {
            var data = _influxGetService.GetData(bucket, organization, startTime, endTime);
            _logger.LogInformation("GetData succeeded. Data count: {Count}", data.Count);
            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching data from InfluxDB");
            return StatusCode(500, "Internal Server Error");
        }
    }


    public void Dispose()
    {
        _client?.Dispose();
    }
}