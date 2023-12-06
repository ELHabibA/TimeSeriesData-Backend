using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;


public class InfluxWriterService : IInfluxWriterService
{
    private readonly InfluxDBClient _client;
    
    public InfluxWriterService(InfluxDBClient influxDbClient)
    {
        _client = influxDbClient ?? throw new ArgumentNullException(nameof(influxDbClient));
        
    }

    public void WriteData(List<string> lineProtocolDataList, string bucket, string organization, string precision = "s")
    {

        try
        {
            using (var writeApi = _client.GetWriteApi())
            {
              foreach (var lineProtocolData in lineProtocolDataList)
              {
                // Write each line to InfluxDB
                writeApi.WriteRecord($"{lineProtocolData}",  MapStringToWritePrecision(precision), bucket, organization);
                
             }
            }
        }
        catch (Exception ex)
        {
            // Handle exception appropriately, e.g., log or rethrow
            throw new Exception("Failed to insert data into InfluxDB.", ex);
        }
    }
    private WritePrecision MapStringToWritePrecision(string precision)
    {
        switch (precision.ToLower())
        {
            case "ms":
                return WritePrecision.Ms;
            case "us":
                return WritePrecision.Us;
            case "ns":
                return WritePrecision.Ns;
            default:
                return WritePrecision.S;
        }
    }

}