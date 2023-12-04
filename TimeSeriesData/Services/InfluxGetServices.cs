using InfluxDB.Client;
using InfluxDB.Client.Core.Flux.Domain;
using System.Collections.Generic;

namespace TimeSeriesData.Services
{
    public class InfluxGetServices : IInfluxGetServices
    {
        private readonly InfluxDBClient _client;

        public InfluxGetServices(InfluxDBClient client)
        {
            _client = client;
        }

        public List<string> GetData(string bucket, string organization, DateTime? startTime = null, DateTime? endTime = null)
        {
            var start = startTime?.ToString("o") ?? "-1h";
            var end = endTime?.ToString("o") ?? "now()";
            var query = $"from(bucket: \"{bucket}\") |> range(start: {start}, stop: {end})";
            var tables = _client.GetQueryApi().QueryAsync(query, organization).Result;

            var data = new List<string>();
            foreach (var table in tables)
            {
                foreach (var record in table.Records)
                {
                    data.Add(record.ToString());
                }
            }

            return data;
        }
    }
}