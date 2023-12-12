using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

public class JsonToLineProtocolConverter
{
     public string ConvertJsonToLineProtocol(string jsonText)
    {
        try
        {
            var jsonArray = JArray.Parse(jsonText);

            if (jsonArray == null)
            {
                throw new ArgumentException("Invalid JSON format");
            }

            var lineProtocolList = new List<string>();

            foreach (var item in jsonArray)
            {
                var measurement = item.Value<string>("measurement");
                var fields = ExtractFields(item);
                var tags = ExtractTags(item);
                var timestamp = item.Value<string>("timestamp");

                // Constructing the Line Protocol string
                string lineProtocol = $"{measurement}{ConstructTagsString(tags)}{ConstructFieldsString(fields)} {GetTimestampString(timestamp)}";
                lineProtocolList.Add(lineProtocol);
            }

            // Convert the list to a JSON array string
            var resultJsonArray = JArray.FromObject(lineProtocolList).ToString();

            return resultJsonArray;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error converting JSON to Line Protocol: {ex.Message}");
            return null;
        }
    }

    private Dictionary<string, object> ExtractFields(JToken jsonObject)
    {
        var fields = new Dictionary<string, object>();

        foreach (var field in jsonObject["fields"].Children<JProperty>())
        {
            var fieldName = field.Name;
            var fieldValue = field.Value;
            if (fieldValue.Type == JTokenType.String || fieldValue.Type == JTokenType.Integer || fieldValue.Type == JTokenType.Float)
            {
                fields.Add(fieldName, fieldValue);
            }
        }

        return fields;
    }

    private Dictionary<string, string> ExtractTags(JToken jsonObject)
    {
        var tags = new Dictionary<string, string>();

        foreach (var tag in jsonObject["tags"].Children<JProperty>())
        {
            var tagName = tag.Name;
            var tagValue = tag.Value.ToString();
            if (!string.IsNullOrWhiteSpace(tagValue))
            {
                tags.Add(tagName, tagValue);
            }
        }

        return tags;
    }

    private string ConstructTagsString(Dictionary<string, string> tags)
    {
        if (tags.Count == 0)
        {
            return "";
        }

        var tagsString = new List<string>();
        foreach (var tag in tags)
        {
            tagsString.Add($"{tag.Key}={EscapeTagValue(tag.Value)}");
        }

        return "," + string.Join(",", tagsString);
    }

    private string ConstructFieldsString(Dictionary<string, object> fields)
    {
        if (fields.Count == 0)
        {
            return "";
        }

        var fieldsString = new List<string>();
        foreach (var field in fields)
        {
            fieldsString.Add($"{field.Key}={EscapeFieldValue(field.Value)}");
        }

        return " " + string.Join(",", fieldsString);
    }

    private string EscapeTagValue(string value)
    {
        // Implement any necessary escaping logic here
        return value.Replace(",", "\\,");
    }

    private string EscapeFieldValue(object value)
    {
        // Implement any necessary escaping logic here
        return value.ToString();
    }

    private string GetTimestampString(string timestamp)
    {
        if (DateTime.TryParse(timestamp, out DateTime dateTime))
        {
            // Convert DateTime to Unix timestamp
            long unixTimestamp = (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            return unixTimestamp.ToString();
        }
        else
        {
            Console.WriteLine("Invalid timestamp format");
            return "";
        }
    }
}


