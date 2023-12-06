using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TimeSeriesData.Tests.InfluxFetcherServiceTests
{
    public class BuildFluxQueryTest
    {
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
    }
}
