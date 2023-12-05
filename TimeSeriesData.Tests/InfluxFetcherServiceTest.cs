using Xunit;
using NSubstitute;
using InfluxDB.Client;
using Timeseriesdata.Functions;

public class InfluxFetcherServiceTests
{
    private readonly InfluxDBClient _influxDBClient;
    private readonly InfluxFetcherService _service;

    public InfluxFetcherServiceTests()
    {
        _influxDBClient = Substitute.For<InfluxDBClient>();
        _service = new InfluxFetcherService(_influxDBClient);
    }

    [Fact]
    public void Constructor_ThrowsException_WhenInfluxDBClientIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new InfluxFetcherService(null!));
    }

}