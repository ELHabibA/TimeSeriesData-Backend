using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timeseriesdata.Models;

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
        [FromQuery] string measurement)
    {
        try
        {
            _logger.LogInformation($"Received request for tags with parameters: organization={organization}, bucket={bucket}, measurement={measurement}");

            // Fetch tags for the given measurement using the existing InfluxFetcherService
            var tags = await _influxFetcherService.GetTagsForMeasurementAsync(organization, bucket, measurement);

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
