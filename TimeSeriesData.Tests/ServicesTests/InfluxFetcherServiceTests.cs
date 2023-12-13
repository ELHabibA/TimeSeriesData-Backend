using System.Text.RegularExpressions;

public class InfluxFetcherServiceTests
{

// BuildFluxQuery


// BuildFluxQuery
    [Fact]
    public void BuildFluxQuery_ReturnsCorrectQuery()
    {
        // Arrange
        var bucket = "testBucket";
        var measurement = "testMeasurement";
        var startTime = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endTime = new DateTime(2022, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        long startUnixTime = ((DateTimeOffset)startTime).ToUnixTimeSeconds();
        long endUnixTime = ((DateTimeOffset)endTime).ToUnixTimeSeconds();
        var coco = InfluxFetcherService.BuildFluxQuery(bucket, measurement, startTime, endTime);

        var expectedQuery = $@"from(bucket: ""{bucket}"")
        |> range(start: {startUnixTime}, stop: {endUnixTime})
        |> filter(fn: (r) => r._measurement == ""{measurement}"")";

        // Normalize whitespace in expectedQuery and coco
        var expectedNormalized = Regex.Replace(expectedQuery.Trim(), @"\s+", " ");
        var actualNormalized = Regex.Replace(coco.Trim(), @"\s+", " ");

        // Assert
        Assert.Equal(expectedNormalized, actualNormalized, StringComparer.OrdinalIgnoreCase);
    }
    
}
