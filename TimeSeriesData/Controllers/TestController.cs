using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timeseriesdata.Functions;

[ApiController]
[Route("api/[controller]")]
public class InfluxTagController : ControllerBase
{
    private readonly IInfluxFetcherService _influxFetcherService;
    private readonly ILogger<InfluxTagController> _logger;

    public InfluxTagController(IInfluxFetcherService influxFetcherService, ILogger<InfluxTagController> logger)
    {
        _influxFetcherService = influxFetcherService ?? throw new ArgumentNullException(nameof(influxFetcherService));
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetTags(
        [FromQuery] string organization,
        [FromQuery] string bucket,
        [FromQuery] string measurement,
        [FromQuery] string startTimeString,
        [FromQuery] string? endTimeString)
    {
        try
        {

             // Parse the time strings
            DateTime startTime = InfluxDbUtilities.ParseTime(startTimeString) ?? DateTime.Now;
            DateTime? endTime = InfluxDbUtilities.ParseTime(endTimeString!);

            _logger.LogInformation($"Received request for tags with parameters: organization={organization}, bucket={bucket}, measurement={measurement}");

            // Fetch tags for the given measurement using the new GetTagsAsync method
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
}