
using Timeseriesdata.Models;


public interface IInfluxFetcherService
{
    Task<List<InfluxDataModel>> GetDataAsync(string organization, string bucket, string measurement, DateTime startTime, DateTime? endTime);

    Task<List<string>> GetTagsAsync(string organization, string bucket, string measurement, DateTime startTime, DateTime? endTime);

    Task<List<InfluxDataModel>> GetDataByTagSetAsync(string organization, string bucket, string measurement, DateTime startTime, DateTime? endTime, Dictionary<string, string> tagSet);
}

