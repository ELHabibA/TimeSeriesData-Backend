using System.Text.RegularExpressions;
using Timeseriesdata.Functions;
public class InfluxFetcherServiceTests
{

// GetDataAsync



//GetTagsAsync



//GetDataByTagSetAsync


    // BuildFluxQuery
    [Fact]
    public void BuildFluxQuery_ReturnsCorrectQuery()
    {
        // Arrange
        var bucket = "testBucket";
        var measurement = "testMeasurement";
        var startTime = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endTime = new DateTime(2022, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        var coco = InfluxFetcherService.BuildFluxQuery(bucket, measurement, startTime, endTime);

        var expectedQuery = BuildExpectedFluxQuery(bucket, measurement, startTime, endTime);

        // Assert
        Assert.Equal(expectedQuery, coco, StringComparer.OrdinalIgnoreCase);
    }


    private string BuildExpectedFluxQuery(string bucket, string measurement, DateTime startTime, DateTime? endTime)
    {
        var fluxQuery = $@"
        from(bucket: ""{bucket}"")
        |> range(start: {InfluxDbUtilities.ToInfluxTimestamp(startTime)}, stop: {InfluxDbUtilities.ToInfluxTimestamp(endTime)})
        |> filter(fn: (r) => r._measurement == ""{measurement}"")";

        return fluxQuery;
    }
    
}
