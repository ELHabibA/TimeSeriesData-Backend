using InfluxDB.Client.Core.Flux.Domain;
using System.Collections.Generic;

namespace TimeSeriesData.Services
{
    public interface IInfluxGetServices
    {
            List<string> GetData(string bucket, string organization, DateTime? startTime = null, DateTime? endTime = null);
        
    }
}