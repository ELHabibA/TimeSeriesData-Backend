using System;
using Xunit;

namespace TimeSeriesData.Tests
{
    public class WeatherForecastTests
    {
        [Fact]
        public void TemperatureF_CalculatesCorrectly()
        {
            // Arrange
            var weatherForecast = new WeatherForecast
            {
                TemperatureC = 20
            };

            // Act
            var temperatureF = weatherForecast.TemperatureF;

            // Assert
            Assert.Equal(67, temperatureF);
        }
    }
}
