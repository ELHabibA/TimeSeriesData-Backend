using Microsoft.AspNetCore.Mvc;
using Timeseriesdata.Functions;

[ApiController]
[Route("api/[controller]")]
public class InfluxTagsController : ControllerBase
{
    private readonly IInfluxFetcherService _influxFetcherService;
    private readonly ILogger<InfluxTagsController> _logger;

    public InfluxTagsController(IInfluxFetcherService influxFetcherService, ILogger<InfluxTagsController> logger)
    {
        _influxFetcherService = influxFetcherService ?? throw new ArgumentNullException(nameof(influxFetcherService));
        _logger = logger;
    }

    [HttpGet("gettags")]
    public async Task<IActionResult> GetTags(
        [FromQuery] string organization,
        [FromQuery] string bucket,
        [FromQuery] string measurement,
        [FromQuery] string startTimeString,
        [FromQuery] string? endTimeString)
    {
        try
        {
            DateTime startTime = InfluxDbUtilities.ParseTime(startTimeString) ?? DateTime.Now;
            DateTime? endTime = InfluxDbUtilities.ParseTime(endTimeString!);

            _logger.LogInformation($"Received request for tags with parameters: organization={organization}, bucket={bucket}, measurement={measurement}");

            // Fetch tags for the given measurement
            var tags = await _influxFetcherService.GetTagsAsync(organization, bucket, measurement, startTime, endTime);

            _logger.LogInformation("Request processed successfully.");

            return Ok(tags);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");

            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("getDatabytags")]
    public async Task<IActionResult> GetDataByTagSet(
        [FromQuery] string organization,
        [FromQuery] string bucket,
        [FromQuery] string measurement,
        [FromQuery] string startTimeString,
        [FromQuery] string? endTimeString,
        [FromQuery] string[]? keys,
        [FromQuery] string[]? values)
    {
        try
        {
            DateTime startTime = InfluxDbUtilities.ParseTime(startTimeString) ?? DateTime.Now;
            DateTime? endTime = InfluxDbUtilities.ParseTime(endTimeString!);

            if (keys != null && values != null && keys.Length == values.Length)
            {
                Dictionary<string, string> tagSet = new Dictionary<string, string>();
                for (int i = 0; i < keys.Length; i++)
                {
                    tagSet.Add(keys[i], values[i]);
                }

                _logger.LogInformation($"Executing GetByTagSet logic with tagSet={string.Join(", ", tagSet)}");

                var result = await _influxFetcherService.GetDataByTagSetAsync(organization, bucket, measurement, startTime, endTime ?? DateTime.Now, tagSet);

                _logger.LogInformation("Request processed successfully.");

                return Ok(result);
            }
            else
            {
                return BadRequest("The number of keys must be equal to the number of values.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");

            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


}