using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSeriesData.Tests.InfluxFetcherServiceTests
{
    public class ParseFluxTablesTest
    {
        [Fact]
        public void ParseFluxTables_ReturnsCorrectDataModels()
        {
            // Arrange
            var fluxTables = new List<FluxTable>
        {
            new FluxTable
            {
                Records = new List<FluxRecord>
                {
                    new FluxRecord
                    {
                        Measurement = "testMeasurement",
                        Fields = new Dictionary<string, object> { { "field1", "value1" } },
                        Tags = new Dictionary<string, string> { { "tag1", "value1" } },
                        Time = DateTime.Now
                    }
                }
            }
        };

            // Act
            var result = ParseFluxTables(fluxTables);

            // Assert
            Assert.Single(result);
            Assert.Equal("testMeasurement", result[0].Measurement);
            Assert.Equal("value1", result[0].Fields!["field1"]);
            Assert.Equal("value1", result[0].Tags!["tag1"]);
        }

        private static List<InfluxDataModel> ParseFluxTables(List<FluxTable> fluxTables)
        {
            var result = new List<InfluxDataModel>();

            foreach (var fluxTable in fluxTables)
            {
                foreach (var fluxRecord in fluxTable.Records!)
                {
                    var dataModel = new InfluxDataModel
                    {
                        Measurement = fluxRecord.Measurement,
                        Fields = fluxRecord.Fields ?? new Dictionary<string, object>(),
                        Tags = fluxRecord.Tags ?? new Dictionary<string, string>(),
                        Timestamp = fluxRecord.Time,
                    };

                    result.Add(dataModel);
                }
            }

            return result;
        }

        private class FluxTable
        {
            public List<FluxRecord>? Records { get; set; }
        }

        private class FluxRecord
        {
            public string? Measurement { get; set; }
            public Dictionary<string, object>? Fields { get; set; }
            public Dictionary<string, string>? Tags { get; set; }
            public DateTime Time { get; set; }
        }

        private class InfluxDataModel
        {
            public string? Measurement { get; set; }
            public Dictionary<string, object>? Fields { get; set; }
            public Dictionary<string, string>? Tags { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
