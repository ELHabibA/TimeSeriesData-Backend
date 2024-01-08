using InfluxDB.Client.Api.Domain;


public class InfluxWriterServiceTests
{



// WriteData



// MapStringToWritePrecision
    [Theory]
    [InlineData("ms", WritePrecision.Ms)]
    [InlineData("us", WritePrecision.Us)]
    [InlineData("ns", WritePrecision.Ns)]
    [InlineData("s", WritePrecision.S)] 
    public void MapStringToWritePrecision_ReturnsCorrectPrecision(string input, WritePrecision expected)
    {
        // Act
        var result = InfluxWriterService.MapStringToWritePrecision(input);

        // Assert
        Assert.Equal(expected, result);
    }


    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    public void MapStringToWritePrecision_InvalidPrecision_ThrowsArgumentException(string? invalidPrecision)
    {
        // Act and Assert
        Assert.Throws<ArgumentException>(() => InfluxWriterService.MapStringToWritePrecision(invalidPrecision!));
    }
}