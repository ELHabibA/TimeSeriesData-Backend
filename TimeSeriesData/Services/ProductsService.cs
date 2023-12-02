using InfluxDB.Client;
using Timeseriesdata.Models;
using Timeseriesdata.Functions;

public class ProductsService : IProductsService
{
    private readonly InfluxDBClient _influxDBClient;
    private readonly ILogger<IProductsService> _logger;

    public ProductsService(InfluxDBClient influxDBClient, ILogger<ProductsService> logger)
    {
        _influxDBClient = influxDBClient ?? throw new ArgumentNullException(nameof(influxDBClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<Product>> GetProductSalesData(DateTime? startTime, DateTime? endTime, string? productName = null)
    {
        try
        {
            string fluxQuery = $@"
                from(bucket: ""Products_Sales"")
                {InfluxDbUtilities.GetTimeRangeFilter(startTime, endTime)}
                |> filter(fn: (r) => r[""_measurement""] == ""product_sales"")
                |> filter(fn: (r) => r[""_field""] == ""units_sold"")";

            if (!string.IsNullOrEmpty(productName))
            {
                fluxQuery += $@" |> filter(fn: (r) => r[""product_name""] == ""{productName}"")";

            }

            fluxQuery += $@"
                |> group(columns: [""product_name""])
                |> aggregateWindow(every: 1m, fn: mean, createEmpty: false)
                |> yield(name: ""mean"")";

            var queryApi = _influxDBClient.GetQueryApi();
            var queryResult = await queryApi.QueryAsync(fluxQuery, "omega").ConfigureAwait(false);

            if (queryResult != null && queryResult.Any())
            {
                var productSalesDataList = new List<Product>();

                foreach (var fluxTable in queryResult)
                {
                    var productSalesData = InfluxDbUtilities.ConvertFluxTableToProductSalesData(fluxTable);
                    productSalesDataList.AddRange(productSalesData);
                }

                return productSalesDataList;
            }
            else
            {
                _logger.LogInformation("No products were found when accessing the bucket!");
                return new List<Product>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error querying InfluxDB: {ex.Message}");
            throw; // Rethrow the exception or handle it accordingly
        }
    }
}
