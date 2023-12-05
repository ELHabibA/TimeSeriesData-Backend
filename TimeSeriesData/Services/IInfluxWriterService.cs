using InfluxDB.Client.Api.Domain;

public interface IInfluxWriterService
{
    void WriteData(
      List<string> lineProtocolDataList, 
      string bucket, 
      string organization, 
      string precision = "s");
    
}
