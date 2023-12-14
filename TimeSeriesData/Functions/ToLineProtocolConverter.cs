using Timeseriesdata.Models;
using Timeseriesdata.Functions;

public class ToLineProtocolConverter
{
    public static List<string> ConvertToLineProtocol(List<InfluxDataModelForCreationDto> influxDataList)
    {
        try
        {
            var lineProtocolList = new List<string>();

            foreach (var influxData in influxDataList)
            {
                var measurement = influxData.Measurement;
                var fields = ExtractFields(influxData);
                var tags = ExtractTags(influxData);
                var timestamp = InfluxDbUtilities.ToInfluxTimestamp(influxData.Timestamp);

                // Constructing the Line Protocol string
                string lineProtocol = $"{measurement}{ConstructTagsString(tags)}{ConstructFieldsString(fields)}";
                if (timestamp != 0)
                {
                    lineProtocol += $" {GetTimestampString(timestamp.ToString())}";
                }
                lineProtocolList.Add(lineProtocol);
            }

            return lineProtocolList;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to Convert To Line Protocol.", ex);
        }
    }

    private static string ExtractFields(InfluxDataModelForCreationDto influxData)
    {
        var fields = new List<string>();

        if (influxData.Fields != null)
        {
            foreach (var field in influxData.Fields)
            {
                fields.Add($"{field.Key}={EscapeFieldValue(field.Value)}");
            }
        }

        return string.Join(",", fields);
    }

    private static string ExtractTags(InfluxDataModelForCreationDto influxData)
    {
        var tags = new List<string>();

        if (influxData.Tags != null)
        {
            foreach (var tag in influxData.Tags)
            {
                tags.Add($"{tag.Key}={EscapeTagValue(tag.Value)}");
            }
        }

        return string.Join(",", tags);
    }

    private static string ConstructTagsString(string tags)
    {
        return string.IsNullOrWhiteSpace(tags) ? "" : $",{tags}";
    }

    private static string ConstructFieldsString(string fields)
    {
        return string.IsNullOrWhiteSpace(fields) ? "" : $" {fields}";
    }

    private static string EscapeTagValue(string value)
    {
        // Implement any necessary escaping logic here
        return value.Replace(",", "\\,");
    }

    private static string EscapeFieldValue(object value)
    {
        // Implement any necessary escaping logic here
        return value.ToString()!;
    }

    private static string GetTimestampString(string timestamp)
    {
        return string.IsNullOrWhiteSpace(timestamp) ? "" : $"{timestamp}";
    }
}
