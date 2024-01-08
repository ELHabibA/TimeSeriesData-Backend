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
            throw new Exception("Failed to insert data into InfluxDB.", ex);
        }
    }
    public static WritePrecision MapStringToWritePrecision(string precision)
    {
        return precision.ToLower() switch
        {
            "ms" => WritePrecision.Ms,
            "us" => WritePrecision.Us,
            "ns" => WritePrecision.Ns,
            "s" => WritePrecision.S,
            _ => throw new ArgumentException($"Invalid precision value: {precision}"),
        };
    }

}