
using Microsoft.AspNetCore.Mvc;
using Timeseriesdata.Functions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

[ApiController]
[Route("api/[controller]")]
public class InfluxFetcherController : ControllerBase
{
    private readonly IInfluxFetcherService _influxDataService;
    private readonly ILogger<InfluxFetcherController> _logger;

    public InfluxFetcherController(IInfluxFetcherService influxDataService, ILogger<InfluxFetcherController> logger)
    {
        _influxDataService = influxDataService ?? throw new ArgumentNullException(nameof(influxDataService));
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
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

            _logger.LogInformation($"Received request with parameters: organization={organization}, bucket={bucket}, measurement={measurement}, startTime={startTime}, endTime={endTime}");

            var result = await _influxDataService.GetDataAsync(organization, bucket, measurement, startTime, endTime ?? DateTime.Now);

            _logger.LogInformation("Request processed successfully.");

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");

            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }



    [HttpGet("byTagSet")]
public async Task<IActionResult> GetByTagSet(
    [FromQuery] string organization,
    [FromQuery] string bucket,
    [FromQuery] string measurement,
    [FromQuery] string startTimeString,
    [FromQuery] string? endTimeString,
    [FromQuery] string[] keys,
    [FromQuery] string[] values)
{
    try
    {
        DateTime startTime = InfluxDbUtilities.ParseTime(startTimeString) ?? DateTime.Now;
        DateTime? endTime = InfluxDbUtilities.ParseTime(endTimeString!);

        if (keys.Length != values.Length)
        {
            return BadRequest("The number of keys must be equal to the number of values.");
        }

        Dictionary<string, string> tagSet = new Dictionary<string, string>();

        for (int i = 0; i < keys.Length; i++)
        {
            tagSet.Add(keys[i], values[i]);
        }

        _logger.LogInformation($"Received request with parameters: organization={organization}, bucket={bucket}, measurement={measurement}, startTime={startTime}, endTime={endTime}, tagSet={string.Join(", ", tagSet)}");

        var result = await _influxDataService.GetDataByTagSetAsync(organization, bucket, measurement, startTime, endTime ?? DateTime.Now, tagSet);

        _logger.LogInformation("Request processed successfully.");

        return Ok(result);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while processing the request.");

        return StatusCode(500, $"Internal Server Error: {ex.Message}");
    }
}

  
}
