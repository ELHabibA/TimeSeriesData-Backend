using Xunit;
using NSubstitute;
using InfluxDB.Client;
using Timeseriesdata.Functions;
using Timeseriesdata.Models;
using NodaTime;

public class ConvertDateTest
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
}