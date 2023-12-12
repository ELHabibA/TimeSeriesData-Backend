using System;
using Xunit;

public class JsonToLineProtocolConverterTests
{
    [Fact]
    public void ConvertJsonToLineProtocol_ValidData_ReturnsExpectedJsonArray()
    {
        // Arrange
        var converter = new JsonToLineProtocolConverter();
        var jsonData = GetDummyData();
        var expectedJsonArray = "[\n  \"products_sales,product_name=7up units_sold=201701935877\",\n  \"products_sales,product_name=7up units_sold=20 1701946138\",\n  \"products_sales,product_name=Fanta units_sold=10 1701935877\"\n]";

        // Act
        var result = converter.ConvertJsonToLineProtocol(jsonData);

        // Assert
        Assert.Equal(expectedJsonArray, result);
    }

    private string GetDummyData()
    {
        // Replace this with your actual dummy data
        return @"[
            {
                ""measurement"": ""products_sales"",
                ""fields"": {
                    ""units_sold"": 20
                },
                ""tags"": {
                    ""product_name"": ""7up""
                },
                ""timestamp"": ""2023-12-07T08:57:57Z""
            },
            {
                ""measurement"": ""products_sales"",
                ""fields"": {
                    ""units_sold"": 20
                },
                ""tags"": {
                    ""product_name"": ""7up""
                },
                ""timestamp"": ""2023-12-07T11:48:58Z""
            },
            {
                ""measurement"": ""products_sales"",
                ""fields"": {
                    ""units_sold"": 10
                },
                ""tags"": {
                    ""product_name"": ""Fanta""
                },
                ""timestamp"": ""2023-12-07T08:57:57Z""
            },
            {
                ""measurement"": ""products_sales"",
                ""fields"": {
                    ""units_sold"": 10
                },
                ""tags"": {
                    ""product_name"": ""Fanta""
                },
                ""timestamp"": ""2023-12-07T11:48:58Z""
            },
            {
                ""measurement"": ""products_sales"",
                ""fields"": {
                    ""units_sold"": 15
                },
                ""tags"": {
                    ""product_name"": ""Sprit""
                },
                ""timestamp"": ""2023-12-07T08:57:57Z""
            },
            {
                ""measurement"": ""products_sales"",
                ""fields"": {
                    ""units_sold"": 15
                },
                ""tags"": {
                    ""product_name"": ""Sprit""
                },
                ""timestamp"": ""2023-12-07T11:48:58Z""
            }
        ]";
    }
}

