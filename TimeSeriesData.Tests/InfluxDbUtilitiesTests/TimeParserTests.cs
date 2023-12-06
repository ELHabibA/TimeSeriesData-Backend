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

        // Round milliseconds to zero
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
    [InlineData("-1w", 7)]
    [InlineData("-2w", 14)] 
    [InlineData("-1d", 1)]
    [InlineData("-2d", 2)]
    public void ParseTime_ReturnsDateTimeAgo_WhenInputIsTimeAgo(string input, int expectedAmount)
    {
        // Act
        var result = InfluxDbUtilities.ParseTime(input);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<DateTime>(result);

       var expectedDateTime = DateTime.UtcNow.Add(TimeSpan.FromDays(-expectedAmount));

        // Round milliseconds to zero
        expectedDateTime = expectedDateTime.AddTicks(-expectedDateTime.Ticks % TimeSpan.TicksPerSecond);
        result = result?.AddTicks(-result.Value.Ticks % TimeSpan.TicksPerSecond);

        Assert.Equal(expectedDateTime, result);
    }


    [Theory]
    [InlineData("-30m", 30)]
    [InlineData("-60m", 60)]
    public void ParseTime_ReturnsDateTimeAgo_WhenInputIsTimeAgoInMinutes(string input, int expectedAmount)
    {
        // Act
        var result = InfluxDbUtilities.ParseTime(input);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<DateTime>(result);

        if (result.HasValue)
        {
            var expectedDateTime = DateTime.UtcNow.AddMinutes(-expectedAmount);

            // Set the expectedDateTime to the exact minute, ignoring seconds and milliseconds
            expectedDateTime = new DateTime(expectedDateTime.Year, expectedDateTime.Month, expectedDateTime.Day, expectedDateTime.Hour, expectedDateTime.Minute, 0, DateTimeKind.Utc);

            // Set the result to the exact minute, ignoring seconds and milliseconds
            var resultValue = result.Value.AddTicks(-result.Value.Ticks % TimeSpan.TicksPerMinute);

            var tolerance = TimeSpan.FromMinutes(1);
            Assert.InRange(resultValue, expectedDateTime - tolerance, expectedDateTime + tolerance);
        }
        else
        {
            
            Assert.Fail("Unexpected null result");
        }
    }


    [Theory]
    [InlineData("-30s", 30)]
    [InlineData("-60s", 60)]
    public void ParseTime_ReturnsDateTimeAgo_WhenInputIsTimeAgoInSeconds(string input, int expectedAmount)
    {
        // Act
        var result = InfluxDbUtilities.ParseTime(input);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<DateTime>(result);  

        if (result.HasValue)
        {
            var expectedDateTime = DateTime.UtcNow.AddSeconds(-expectedAmount);

            // Set the expectedDateTime to the exact second, ignoring milliseconds
            expectedDateTime = new DateTime(expectedDateTime.Year, expectedDateTime.Month, expectedDateTime.Day, expectedDateTime.Hour, expectedDateTime.Minute, expectedDateTime.Second, DateTimeKind.Utc);

            // Set the result to the exact second, ignoring milliseconds
            var resultValue = result.Value.AddTicks(-result.Value.Ticks % TimeSpan.TicksPerSecond);

            var tolerance = TimeSpan.FromSeconds(1);
            Assert.InRange(resultValue, expectedDateTime - tolerance, expectedDateTime + tolerance);
        }
        else
        {
            // Fail the test with a specific message
            Assert.Fail("Unexpected null result");
        }
    }


    [Theory]
    [InlineData("-1y", 1)]
    [InlineData("-2y", 2)]
    public void ParseTime_ReturnsDateTimeAgo_WhenInputIsTimeAgoInYears(string input, int expectedAmount)
    {
        // Act
        var result = InfluxDbUtilities.ParseTime(input);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<DateTime>(result);

        if (result.HasValue)
        {
            var expectedDateTime = DateTime.UtcNow.AddYears(-expectedAmount);

            // Set the expectedDateTime to the exact hour and minute, ignoring seconds and milliseconds
            expectedDateTime = new DateTime(expectedDateTime.Year, expectedDateTime.Month, expectedDateTime.Day, expectedDateTime.Hour, 0, 0, DateTimeKind.Utc);

            // Set the result to the exact hour and minute, ignoring seconds and milliseconds
            var resultValue = result.Value.AddTicks(-result.Value.Ticks % TimeSpan.TicksPerHour);

            var tolerance = TimeSpan.FromHours(1);
            Assert.InRange(resultValue, expectedDateTime - tolerance, expectedDateTime + tolerance);
        }
        else
        {
            // Fail the test with a specific message
            Assert.Fail("Unexpected null result");
        }
    }



    [Theory]
    [InlineData("-2M", 2)]
    [InlineData("-1M", 1)]
    public void ParseTime_ReturnsDateTimeAgo_WhenInputIsTimeInMonths(string input, int expectedAmount)
    {
        // Act
        var result = InfluxDbUtilities.ParseTime(input);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<DateTime>(result);

        var expectedDateTime = DateTime.UtcNow.AddMonths(-expectedAmount);

        var tolerance = TimeSpan.FromSeconds(1);
        Assert.InRange(result.Value, expectedDateTime - tolerance, expectedDateTime + tolerance);
    }



    [Fact]
    public void ParseTime_AdjustsMonths_PreservesDayOfMonth()
    {
        // Arrange
        var baseDate = new DateTime(2023, 6, 12, 12, 0, 0, DateTimeKind.Utc);

        // Act
        var result = InfluxDbUtilities.AdjustMonths(baseDate, -1);

        // Assert
        var expectedDate = new DateTime(2023, 5, 12, 12, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expectedDate, result);
    }


    [Fact]
    public void ParseTime_AdjustsMonths_PreservesDayOfMonth_NegativeMonths()
    {
        // Arrange
        var baseDate = new DateTime(2023, 6, 12, 12, 0, 0, DateTimeKind.Utc);

        // Act
        var result = InfluxDbUtilities.AdjustMonths(baseDate, -2);

        // Assert
        var expectedDate = new DateTime(2023, 4, 12, 12, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expectedDate, result);
    }

   [Fact]
    public void ParseTime_ThrowsArgumentException_WhenInputIsInvalid()
    {
        // Arrange
        string invalidInput = "invalid";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => InfluxDbUtilities.ParseTime(invalidInput));
    } 
}
