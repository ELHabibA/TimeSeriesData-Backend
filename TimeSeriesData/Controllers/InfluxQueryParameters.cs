using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;


public class InfluxQueryParameters
{
    public string? Organization { get; set; }
    public string? Bucket { get; set; }
    public string? Measurement { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public Dictionary<string, string>? TagSet { get; set; }
}
