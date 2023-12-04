using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Flux.Domain;
using Microsoft.AspNetCore.Mvc;
using Timeseriesdata.Models;
using Timeseriesdata.Functions;
using NodaTime;

    [ApiController]
    [Route("api/[controller]")]
    public class InfluxDataController : ControllerBase
    {
        private readonly InfluxDBClient _influxDBClient;

        public InfluxDataController(InfluxDBClient influxDBClient)
        {
            _influxDBClient = influxDBClient ?? throw new ArgumentNullException(nameof(influxDBClient));
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string organization,
            [FromQuery] string bucket,
            [FromQuery] string measurement,
            [FromQuery] DateTime startTime,
            [FromQuery] DateTime endTime)
        {
            try
            {
                var queryApi = _influxDBClient.GetQueryApi();

                // Build Flux query to fetch data
                var fluxQuery = $@"
                    from(bucket: ""{bucket}"")
                        |> range(start: {ToInfluxTimestamp(startTime)}, stop: {ToInfluxTimestamp(endTime)})";

                if (!string.IsNullOrEmpty(measurement))
                {
                    fluxQuery += $" |> filter(fn: (r) => r._measurement == \"{measurement}\")";
                }

                // Execute query
                var fluxTables = await queryApi.QueryAsync(fluxQuery, organization);

                // Parse and return data
                var result = ParseFluxTables(fluxTables);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        private static long ToInfluxTimestamp(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks / 10000000;
        }

       private List<InfluxDataModel> ParseFluxTables(List<FluxTable> fluxTables)
{
    var result = new List<InfluxDataModel>();

    foreach (var fluxTable in fluxTables)
    {
        foreach (var fluxRecord in fluxTable.Records)
        {
          var  Tag = fluxRecord.Values
        .Where(pair => pair.Value is Dictionary<string, string>)
        .ToDictionary(pair => pair.Key, pair => ((Dictionary<string, string>)pair.Value).Values.FirstOrDefault());
            var dataModel = new InfluxDataModel
            {
                Measurement = fluxRecord.GetMeasurement(),
                Fields = ExtractFields(fluxRecord),
                Tags = Tag,
                Timestamp = ConvertInstantToDateTime(fluxRecord.GetTime()),
                //RawRecord = GetRawRecordString(fluxRecord)
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

            //tagDictionary[key];

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

           private DateTime? ConvertInstantToDateTime(Instant? instant)
          {
                return instant?.InUtc().ToDateTimeUtc();
          }   

    }
