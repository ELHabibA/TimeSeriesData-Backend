using System.ComponentModel.DataAnnotations;


namespace Timeseriesdata.Models
{
    public class InfluxDataModelForCreationDto
    {

    [Required(ErrorMessage = "You should provide a measurement. InfluxDB accepts one measurement per point.")]
    [MaxLength(50)]
    public required string Measurement { get; set; }


    [Required(ErrorMessage = "You should provide a field. Points must have at least one field.")]
    public required Dictionary<string, object> Fields { get; set; }


    public Dictionary<string, string>? Tags { get; set; }
    public DateTime? Timestamp { get; set; }

    }
} 