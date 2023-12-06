using Timeseriesdata.Functions;

public class TimeParserTests
{
    [Fact]
    public void ParseTime_ReturnsNull_WhenInputIsNullOrEmpty()
    {
        // Arrange
        string nullOrEmptyString = null!;

        // Act
        var result = InfluxDbUtilities.ParseTime(nullOrEmptyString);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("now")]
    [InlineData("NOW")]
    [InlineData("NoW")]
    public void ParseTime_ReturnsUtc_WhenInputIsNow(string input)
    {
        // Act
        var result = InfluxDbUtilities.ParseTime(input);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<DateTime>(result);
      }

    [Theory]
    [InlineData("now")]
    [InlineData("NOW")]
    [InlineData("NoW")]
    public void ParseTime_ReturnsUtcNow_WhenInputIsNow(string input)
    {
        // Arrange & Act
        DateTime currentUtcDateTime = DateTime.UtcNow;
        var result = InfluxDbUtilities.ParseTime(input);

        // Round milliseconds to zero for both DateTime objects
        currentUtcDateTime = currentUtcDateTime.AddTicks(-currentUtcDateTime.Ticks % TimeSpan.TicksPerSecond);
        result = result?.AddTicks(-result.Value.Ticks % TimeSpan.TicksPerSecond);

        // Assert
        Assert.Equal(currentUtcDateTime, result);
    }

    [Theory]
    [InlineData("2023-12-06")]
    [InlineData("12/06/2023")]
    [InlineData("2023-12-06T12:34:56")]
    public void ParseTime_ReturnsDateTime_WhenInputIsAValidDateTime(string input)
    {
        // Act
        var result = InfluxDbUtilities.ParseTime(input);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<DateTime>(result);
    }

    [Theory]
   /* [InlineData("-1w", 7)]
    [InlineData("-2w", 14)] 
    [InlineData("-1M", 1)] */
    [InlineData("-2M", 2)]
     /* [InlineData("-1y", 1)]
    [InlineData("-2y", 2)]
    [InlineData("-1h", 1)]
    [InlineData("-2h", 2)]
    [InlineData("-1d", 1)]
    [InlineData("-2d", 2)]
    [InlineData("-1m", 1)]
    [InlineData("-2m", 2)]
    [InlineData("-1s", 1)]
    [InlineData("-2s", 2)]*/
    public void ParseTime_ReturnsDateTimeAgo_WhenInputIsTimeAgo(string input, int expectedAmount)
    {
        // Act
        var result = InfluxDbUtilities.ParseTime(input);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<DateTime>(result);

       var expectedDateTime = DateTime.UtcNow.Add(TimeSpan.FromDays(-expectedAmount));

        // Round milliseconds to zero for both DateTime objects
        expectedDateTime = expectedDateTime.AddTicks(-expectedDateTime.Ticks % TimeSpan.TicksPerSecond);
        result = result?.AddTicks(-result.Value.Ticks % TimeSpan.TicksPerSecond);

        Assert.Equal(expectedDateTime, result);
    }

   /* [Fact]
    public void ParseTime_ThrowsArgumentException_WhenInputIsInvalid()
    {
        // Arrange
        string invalidInput = "invalid";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => InfluxDbUtilities.ParseTime(invalidInput));
    } */
}
