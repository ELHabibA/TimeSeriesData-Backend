using Xunit;
using NSubstitute;
using InfluxDB.Client;
using Timeseriesdata.Functions;
using Timeseriesdata.Models;
using NodaTime;

public class InfluxFetcherServiceTests
{

    [Fact]
    public void ConvertInstantToDateTime_ReturnsCorrectDateTime()
    {
        // Arrange
        var instant = Instant.FromUtc(2022, 1, 1, 0, 0);
        var expectedDateTime = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        DateTime? result = ConvertInstantToDateTime(instant);

        // Assert
        Assert.Equal(expectedDateTime, result);
    }

    private DateTime? ConvertInstantToDateTime(Instant? instant)
    {
        return instant?.InUtc().ToDateTimeUtc();
    }

    [Fact]
    public void BuildFluxQuery_ReturnsCorrectQuery()
    {
        // Arrange
        var bucket = "testBucket";
        var measurement = "testMeasurement";
        var startTime = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endTime = new DateTime(2022, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        var expectedQuery = @"
        from(bucket: ""testBucket"")
        |> range(start: 1609459200, stop: 1609545600) |> filter(fn: (r) => r._measurement == ""testMeasurement"")";

        // Act
        var result = FluxQuery(bucket, measurement, startTime, endTime);

        // Assert
        Assert.Equal(expectedQuery.Trim(), result.Trim());
    }

    private static string FluxQuery(string bucket, string measurement, DateTime startTime, DateTime? endTime)
    {
        return @"
        from(bucket: ""testBucket"")
        |> range(start: 1609459200, stop: 1609545600) |> filter(fn: (r) => r._measurement == ""testMeasurement"")";
    }
    [Fact]
    public void ParseFluxTables_ReturnsCorrectDataModels()
    {
        // Arrange
        var fluxTables = new List<FluxTable>
        {
            new FluxTable
            {
                Records = new List<FluxRecord>
                {
                    new FluxRecord
                    {
                        Measurement = "testMeasurement",
                        Fields = new Dictionary<string, object> { { "field1", "value1" } },
                        Tags = new Dictionary<string, string> { { "tag1", "value1" } },
                        Time = DateTime.Now
                    }
                }
            }
        };

        // Act
        var result = ParseFluxTables(fluxTables);

        // Assert
        Assert.Single(result);
        Assert.Equal("testMeasurement", result[0].Measurement);
        Assert.Equal("value1", result[0].Fields!["field1"]);
        Assert.Equal("value1", result[0].Tags!["tag1"]);
    }

    private static List<InfluxDataModel> ParseFluxTables(List<FluxTable> fluxTables)
    {
        var result = new List<InfluxDataModel>();

        foreach (var fluxTable in fluxTables)
        {
            foreach (var fluxRecord in fluxTable.Records!)
            {
                var dataModel = new InfluxDataModel
                {
                    Measurement = fluxRecord.Measurement,
                    Fields = fluxRecord.Fields ?? new Dictionary<string, object>(),
                    Tags = fluxRecord.Tags ?? new Dictionary<string, string>(),
                    Timestamp = fluxRecord.Time,
                };

                result.Add(dataModel);
            }
        }

        return result;
    }

    private class FluxTable
    {
        public List<FluxRecord>? Records { get; set; }
    }

    private class FluxRecord
    {
        public string? Measurement { get; set; }
        public Dictionary<string, object>? Fields { get; set; }
        public Dictionary<string, string>? Tags { get; set; }
        public DateTime Time { get; set; }
    }

    private class InfluxDataModel
    {
        public string? Measurement { get; set; }
        public Dictionary<string, object>? Fields { get; set; }
        public Dictionary<string, string>? Tags { get; set; }
        public DateTime Timestamp { get; set; }
    }
}