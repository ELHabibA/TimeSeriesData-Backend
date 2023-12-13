using InfluxDB.Client.Api.Domain;


public class InfluxWriterServiceTests
{

// MapStringToWritePrecision
    [Theory]
    [InlineData("ms", WritePrecision.Ms)]
    [InlineData("us", WritePrecision.Us)]
    [InlineData("ns", WritePrecision.Ns)]
    [InlineData("other", WritePrecision.S)] 
    public void MapStringToWritePrecision_ReturnsCorrectPrecision(string input, WritePrecision expected)
    {
        // Act
        var result = InfluxWriterService.MapStringToWritePrecision(input);

        // Assert
        Assert.Equal(expected, result);
    }
}