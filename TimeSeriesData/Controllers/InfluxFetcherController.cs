
using Microsoft.AspNetCore.Mvc;



[ApiController]
[Route("api/[controller]")]
public class InfluxFetcherController : ControllerBase
{
    private readonly IInfluxFetcherService _influxDataService;

    public InfluxFetcherController(IInfluxFetcherService influxDataService)
    {
        _influxDataService = influxDataService ?? throw new ArgumentNullException(nameof(influxDataService));
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string organization,
        [FromQuery] string bucket,
        [FromQuery] string measurement,
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime)
    {
        try
        {
            var result = await _influxDataService.GetDataAsync(organization, bucket, measurement, startTime, endTime);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

}
