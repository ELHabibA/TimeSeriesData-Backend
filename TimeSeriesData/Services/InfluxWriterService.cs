using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;


public class InfluxWriterService : IInfluxWriterService, IDisposable
{
    private readonly InfluxDBClient _client;
    private readonly ILogger<InfluxWriterService> _logger;

    public InfluxWriterService(InfluxDBClient influxDbClient, ILogger<InfluxWriterService> logger)
    {
        _client = influxDbClient ?? throw new ArgumentNullException(nameof(influxDbClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void WriteData(List<string> lineProtocolDataList, string bucket, string organization, WritePrecision precision = WritePrecision.S)
    {

        try
        {
            using (var writeApi = _client.GetWriteApi())
            {
              foreach (var lineProtocolData in lineProtocolDataList)
              {
                // Write each line to InfluxDB
                writeApi.WriteRecord($"{lineProtocolData}", precision, bucket, organization);
                
             }
            }
        }
        catch (Exception ex)
        {
            // Handle exception appropriately, e.g., log or rethrow
            throw new Exception("Failed to insert data into InfluxDB.", ex);
        }
    }


    public void Dispose()
    {
        _client?.Dispose();
    }
}
