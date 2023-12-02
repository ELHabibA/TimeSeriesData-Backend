
namespace Timeseriesdata.Models
{
    public class InfluxDataModel
    {
    public string? Measurement { get; set; }
    public Dictionary<string, object>? Fields { get; set; }
    public Dictionary<string, string>? Tags { get; set; }
    public DateTime Timestamp { get; set; }

    }
}