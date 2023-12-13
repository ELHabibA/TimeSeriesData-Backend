
using NodaTime;
using Timeseriesdata.Functions;

namespace TimeSeriesData.Tests.InfluxFetcherServiceTests
{
    public class ToInfluxTimestampTests
    {
        [Fact]
        public void ConvertInstantToDateTime_ReturnsCorrectDateTime()
        {
            // Arrange
            var instant = Instant.FromUtc(2022, 1, 1, 0, 0);
            var expectedDateTime = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var actualDateTime = InfluxDbUtilities.ConvertInstantToDateTime(instant);

            // Assert
            Assert.Equal(expectedDateTime, actualDateTime);
        }

        [Fact]
        public void ToInfluxTimestamp_ReturnsCorrectTimestamp()
        {
                // Arrange
          var dateTime = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
          var expectedTimestamp = (dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks / 10000000);

                // Act
          var actualTimestamp = InfluxDbUtilities.ToInfluxTimestamp(dateTime);

                // Assert
          Assert.Equal(expectedTimestamp, actualTimestamp);
        }

        [Fact]
        public void ToInfluxTimestamp_NullDateTime_ReturnsUnixEpoch()
        {
            // Arrange
            DateTime? dateTime = null;
            var expectedTimestamp = 0; // Unix time for DateTime.MinValue

            // Act
            var actualTimestamp = InfluxDbUtilities.ToInfluxTimestamp(dateTime);

            // Assert
            Assert.Equal(expectedTimestamp, actualTimestamp);
        }
    }
  }

