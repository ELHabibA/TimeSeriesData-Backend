using Microsoft.AspNetCore.Mvc;
using Timeseriesdata.Functions;
using Timeseriesdata.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class InfluxWriterController : ControllerBase
{
    private readonly IInfluxWriterService _influxWriterService;
    private readonly ILogger<InfluxWriterController> _logger;
   

    public InfluxWriterController(IInfluxWriterService writerService, ILogger<InfluxWriterController> logger)
    {
        _influxWriterService = writerService ?? throw new ArgumentNullException(nameof(writerService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
 
     [HttpPost]
    public IActionResult PostData([FromBody] List<InfluxDataModelForCreationDto>  DataList, [FromQuery] string bucket, [FromQuery] string organization, [FromQuery] string precision = "s")
    {
       // _logger.LogInformation("Received request to insert data. Bucket: {Bucket}, Organization: {Organization}, Precision: {Precision}, Number of records: {Count}",
       //     bucket, organization, precision, lineProtocolDataList.Count);

        try
        {

             var lineProtocolDataList = JsonToLineProtocolConverter.ConvertToLineProtocol(DataList);

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
}