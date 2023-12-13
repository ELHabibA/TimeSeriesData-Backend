using InfluxDB.Client;
using InfluxDB.Client.Core.Flux.Domain;
using Timeseriesdata.Models;
using Timeseriesdata.Functions;


public class InfluxFetcherService : IInfluxFetcherService
{
    private readonly InfluxDBClient _influxDBClient;

    public InfluxFetcherService(InfluxDBClient influxDBClient)
    {
        _influxDBClient = influxDBClient ?? throw new ArgumentNullException(nameof(influxDBClient));
    }

    public async Task<List<InfluxDataModel>> GetDataAsync(string organization, string bucket, string measurement, DateTime startTime, DateTime? endTime)
    {
        var queryApi = _influxDBClient.GetQueryApi();

        var fluxQuery = BuildFluxQuery(bucket, measurement, startTime, endTime);

        var fluxTables = await queryApi.QueryAsync(fluxQuery, organization);

        return ParseFluxTables(fluxTables);
    }

    public static string BuildFluxQuery(string bucket, string measurement, DateTime startTime, DateTime? endTime)
    {
        // If endTime is not provided, set it to "now"
        if(!endTime.HasValue)
              endTime = DateTime.UtcNow;

      var fluxQuery = $@"
        from(bucket: ""{bucket}"")

        |> range(start: {InfluxDbUtilities.ToInfluxTimestamp(startTime)}, stop: {InfluxDbUtilities.ToInfluxTimestamp(endTime)})

        |> range(start: {ToInfluxTimestamp(startTime)}, stop: {ToInfluxTimestamp(endTime)})

        |> filter(fn: (r) => r._measurement == ""{measurement}"")";

        return fluxQuery;
    }

<

        private static long ToInfluxTimestamp(DateTime? dateTime)
        {
            return (dateTime ?? DateTime.MinValue).ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks / 10000000;
        }



       private List<InfluxDataModel> ParseFluxTables(List<FluxTable> fluxTables)
       {
          var result = new List<InfluxDataModel>();

          foreach (var fluxTable in fluxTables)
         {
            foreach (var fluxRecord in fluxTable.Records)
            {
               var dataModel = new InfluxDataModel
              {
                Measurement = fluxRecord.GetMeasurement(),
                Tags = ExtractTags(fluxRecord),
                Fields = ExtractFields(fluxRecord),
                Timestamp = InfluxDbUtilities.ConvertInstantToDateTime(fluxRecord.GetTime()),
             };

            result.Add(dataModel);
           }
         }

             return result;
        }

        private Dictionary<string, string> ExtractTags(FluxRecord fluxRecord)
        {
         var tagDictionary = new Dictionary<string, string>();

          foreach (var keyValue in fluxRecord.Values)
           {
             var key = keyValue.Key;
             var value = keyValue.Value;

            if(key!="result" && key!="table" && key!="_start" && key!="_stop" && key!="_time" && key!="_value" && key!="_field" && key!="_measurement" ){
            tagDictionary[key] = (string) value;
            }
           }

           return tagDictionary;
        }

        private Dictionary<string, object> ExtractFields(FluxRecord fluxRecord)
        {
        var fieldDictionary = new Dictionary<string, object>();

        foreach (var keyValue in fluxRecord.Values)
        {
        var key = fluxRecord.GetField();
        var value = fluxRecord.GetValue();

        fieldDictionary[key] = value;

        }

        return fieldDictionary;
        }
        
}
