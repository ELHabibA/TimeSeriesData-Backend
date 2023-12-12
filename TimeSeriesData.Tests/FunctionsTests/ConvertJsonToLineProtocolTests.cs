
using Timeseriesdata.Models;


public class JsonToLineProtocolConverterTests
{

[Fact]
public void ConvertToLineProtocol_ValidData_ReturnsExpectedList()
{
    // Arrange
    var influxDataList = GetDummyData();
    List<string> expectedList = new List<string>(new string[]
    {
       "products_sales,product_name=7up units_sold=20",
       "products_sales,product_name=Nocco units_sold=30 1701949738",
       "products_sales,product_name=Fanta units_sold=40 1701939477"
    });

    // Act
    var result = JsonToLineProtocolConverter.ConvertToLineProtocol(influxDataList);

    // Output for inspection
    Console.WriteLine("Expected:");
    foreach (var expected in expectedList)
    {
        Console.WriteLine(expected);
    }

    Console.WriteLine("\nActual:");
    foreach (var actual in result)
    {
        Console.WriteLine(actual);
    }

    // Assert
    Assert.Equal(expectedList, result);
}


private List<InfluxDataModelForCreationDto> GetDummyData()
{
        // Replace this with your actual dummy data
        return new List<InfluxDataModelForCreationDto>
        {
            new InfluxDataModelForCreationDto
            {
                Measurement = "products_sales",
                Fields = new Dictionary<string, object>
                {
                    { "units_sold", 20 }
                },
                Tags = new Dictionary<string, string>
                {
                    { "product_name", "7up" }
                }
                
            },

            new InfluxDataModelForCreationDto
            {
                Measurement = "products_sales",
                Fields = new Dictionary<string, object>
                {
                    { "units_sold", 30 }
                },
                Tags = new Dictionary<string, string>
                {
                    { "product_name", "Nocco" }
                },
                Timestamp = DateTime.Parse("2023-12-07T11:48:58Z")
            },
            new InfluxDataModelForCreationDto
            {
                Measurement = "products_sales",
                Fields = new Dictionary<string, object>
                {
                    { "units_sold", 40 }
                },
                Tags = new Dictionary<string, string>
                {
                    { "product_name", "Fanta" }
                },
                Timestamp = DateTime.Parse("2023-12-07T08:57:57Z")
            }
        };
}
}


