using Microsoft.AspNetCore.Mvc;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Timeseriesdata.Functions;

[Route("api")]
[ApiController]
public class ProductsController : ControllerBase
{
     private readonly ILogger<ProductsController> _logger;
    private readonly IProductsService _ProductsService;
    private readonly InfluxDBClient _client;

    public ProductsController(IProductsService influxDbService, ILogger<ProductsController> logger, InfluxDBClient influxDbClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ProductsService = influxDbService ?? throw new ArgumentNullException(nameof(influxDbService));
        _client = influxDbClient ?? throw new ArgumentNullException(nameof(influxDbClient));
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetProductSalesDataWithOptionalParams(
        [FromQuery] string productName = null!,
        [FromQuery] string startTime = null!,
        [FromQuery] string endTime = null!)
    {
        DateTime? startDateTime = InfluxDbUtilities.ParseTime(startTime);
        DateTime? endDateTime = InfluxDbUtilities.ParseTime(endTime);

        if (!string.IsNullOrEmpty(productName))
        {
            return await GetProductSalesDataInternal(productName, startDateTime, endDateTime);
        }

        var productSalesData = await _ProductsService.GetProductSalesData(startDateTime, endDateTime);

        if (productSalesData.Any())
        {
            var formattedProductSalesDataList = productSalesData
                .Select(p => new
                {
                    p.Product_Name,
                    p.Units_Sold,
                    Time = p.Time.ToString("yyyy-MM-ddTHH:mm:ss")
                })
                .ToList();

            return Ok(formattedProductSalesDataList);
        }
        else
        {
            _logger.LogInformation("No data found for the given time");
            return NotFound("No data found for the given time");
        }
    }

    private async Task<IActionResult> GetProductSalesDataInternal(string productName, DateTime? startTime, DateTime? endTime)
    {
        try
        {
            var productSalesData = await _ProductsService.GetProductSalesData(startTime, endTime, productName);

            if (productSalesData.Any())
            {
                var formattedProductSalesDataList = productSalesData
                    .Select(p => new
                    {
                        p.Product_Name,
                        p.Units_Sold,
                        Time = p.Time.ToString("yyyy-MM-ddTHH:mm:ss")
                    })
                    .ToList();

                return Ok(formattedProductSalesDataList);
            }
            else
            {
                return NotFound($"No data found for the product '{productName}' and the given time");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


     [HttpPost("insert")]
    public IActionResult InsertProductSalesData([FromBody] List<string> lineProtocolDataList, [FromQuery] string organization = "omega")
    {
        try
        {
            using var writeApi = _client.GetWriteApi();

            string bucket = "Products_Sales";

            foreach (var lineProtocolData in lineProtocolDataList)
            {
                _logger.LogInformation("InsertProductSalesData called with {count} items", lineProtocolDataList.Count);
                _logger.LogInformation($"InsertProductSalesData called with {lineProtocolData} items");

                // Write each line to InfluxDB
                writeApi.WriteRecord($"{lineProtocolData}", WritePrecision.S, bucket, organization);

                // Log the actual data being written
                _logger.LogInformation($"Writing data: {organization},{bucket},{lineProtocolData}");
            }

            return Ok("Data inserted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while inserting data into InfluxDB.");
            return StatusCode(500, "Internal Server Error");
        }
    }
}