using InfluxDB.Client.Api.Domain;

public interface IInfluxWriterServices
{
    void WriteData(
      List<string> lineProtocolDataList, 
      string bucket, 
      string organization, 
      WritePrecision precision = WritePrecision.S);
    
}
