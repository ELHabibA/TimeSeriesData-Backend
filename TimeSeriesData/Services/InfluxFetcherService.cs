using InfluxDB.Client;
using InfluxDB.Client.Core.Flux.Domain;
using Timeseriesdata.Models;
using Timeseriesdata.Functions;



public class InfluxFetcherService : IInfluxFetcherService
{
    private readonly InfluxDBClient _influxDBClient;
    private readonly ILogger<InfluxFetcherService> _logger;

    public InfluxFetcherService(InfluxDBClient influxDBClient, ILogger<InfluxFetcherService> logger)
    {
        _influxDBClient = influxDBClient ?? throw new ArgumentNullException(nameof(influxDBClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<InfluxDataModel>> GetDataAsync(string organization, string bucket, string measurement, DateTime startTime, DateTime? endTime)
    {
        var queryApi = _influxDBClient.GetQueryApi();

        var fluxQuery = BuildFluxQuery(bucket, measurement, startTime, endTime);

        var fluxTables = await queryApi.QueryAsync(fluxQuery, organization);

        return ParseFluxTables(fluxTables);
    }


    public async Task<List<string>> GetTagsAsync(string organization, string bucket, string measurement, DateTime startTime, DateTime? endTime)
    {
        var queryApi = _influxDBClient.GetQueryApi();

        var fluxQuery = $@"
        from(bucket: ""{bucket}"")
        |> range(start: 0)
        |> filter(fn: (r) => r._measurement == ""{measurement}"")
        |> keys()";

        var fluxTables = await queryApi.QueryAsync(fluxQuery, organization);
        return ParseFluxTags(fluxTables);
    }

    public async Task<List<string>> GetPossibleTagsValuesAsync(string organization, string bucket, string measurement, DateTime startTime, DateTime? endTime)
    {
        var queryApi = _influxDBClient.GetQueryApi();

        var fluxQuery = $@"
        from(bucket: ""{bucket}"")
        |> range(start: 0)
        |> filter(fn: (r) => r._measurement == ""{measurement}"")
        |> keys()";

        var fluxTables = await queryApi.QueryAsync(fluxQuery, organization);
        return ParseFluxTagsAndPossibleValues(fluxTables);
    }

    public async Task<List<InfluxDataModel>> GetDataByTagSetAsync(string organization, string bucket, string measurement, DateTime startTime, DateTime? endTime, Dictionary<string, string> tagSet)
    {
            var queryApi = _influxDBClient.GetQueryApi();
            List<InfluxDataModel> data = new List<InfluxDataModel>();
            var fluxQuery = BuildFluxQuery(bucket, measurement, startTime, endTime);

            try
            {
                 var fluxTables = await queryApi.QueryAsync(fluxQuery, organization);

                foreach (var item in ParseFluxTables(fluxTables))
                {

                    if (item.Tags != null && tagSet != null &&
                        item.Tags.OrderBy(kvp => kvp.Key).SequenceEqual(tagSet.OrderBy(kvp => kvp.Key)))
                    {
                        data.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while querying data by tag set.");
            }

            _logger.LogInformation($"Retrieved {data.Count} data points using tag set.");
            Console.WriteLine(data.Count);

            return data;
    }


    public static string BuildFluxQuery(string bucket, string measurement, DateTime startTime, DateTime? endTime)
    {
        // If endTime is not provided, set it to "now"
        if(!endTime.HasValue)
                endTime = DateTime.UtcNow;

        var fluxQuery = $@"
        from(bucket: ""{bucket}"")
        |> range(start: {InfluxDbUtilities.ToInfluxTimestamp(startTime)}, stop: {InfluxDbUtilities.ToInfluxTimestamp(endTime)})
        |> filter(fn: (r) => r._measurement == ""{measurement}"")";

        return fluxQuery;
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

        if(key!="result" && key!="table" && key!="_start" && key!="_stop" && key!="_time" 
           && key!="_value" && key!="_field" && key!="_measurement" ){

        tagDictionary[key] = (string) value;

        }
        }

        return tagDictionary;
    }

    private Dictionary<string, object> ExtractFields(FluxRecord fluxRecord)
    {
        var fieldDictionary = new Dictionary<string, object>();

        for (int i = 0; i < fluxRecord.Values.Count; i++)
        {
            var key = fluxRecord.GetField();
            var value = fluxRecord.GetValue();

            fieldDictionary[key] = value;

        }

        return fieldDictionary;
    }


    private List<string> ParseFluxTags(List<FluxTable> fluxTables)
    {
        var result = new List<string>();

        foreach (var fluxTable in fluxTables)
        {
            foreach (var fluxRecord in fluxTable.Records)
            {
                foreach (var keyValue in fluxRecord.Values)
                {
                    var key = keyValue.Key;

                    // Check if the key is a tag key
                    if (key!="result" && key!="table" && key!="_start" && key!="_stop" && key!="_time" 
                       && key!="_value" && key!="_field" && key!="_measurement" )
                    {
                        result.Add(key);
                    }
                }
            }
        }

        // Remove duplicate tag keys if needed
        result = result.Distinct().ToList();

        return result;
    }
        
     private List<string> ParseFluxTagsAndPossibleValues(List<FluxTable> fluxTables)
    {
       var result = new List<string>();

        foreach (var fluxTable in fluxTables)
        {
            foreach (var fluxRecord in fluxTable.Records)
            {
                foreach (var keyValue in fluxRecord.Values)
                {
                    var key = keyValue.Key;
                    var value = keyValue.Value;

                    // Check if the key is a tag key
                    if (key!="result" && key!="table" && key!="_start" && key!="_stop" && key!="_time" 
                       && key!="_value" && key!="_field" && key!="_measurement" )
                    {
                         result.Add((string)value);
                    }
                }
            }
        }

        // Remove duplicate tag keys if needed
       result = result.Distinct().ToList();


        return result;
    }
}
