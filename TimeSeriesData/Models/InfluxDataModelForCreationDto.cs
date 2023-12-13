namespace Timeseriesdata.Models
{
    public class InfluxDataModelForCreationDto
    {
    public string? Measurement { get; set; }
    public Dictionary<string, object>? Fields { get; set; }
    public Dictionary<string, string>? Tags { get; set; }
    public DateTime? Timestamp { get; set; }

    }
}