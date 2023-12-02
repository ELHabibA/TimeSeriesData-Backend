
using InfluxDB.Client.Core.Flux.Domain;
using Timeseriesdata.Models;

namespace Timeseriesdata.Functions
{

    public class InfluxDbUtilities
    {
        public static List<Product> ConvertFluxTableToProductSalesData(FluxTable fluxTable)
        {
            var productSalesDataList = new List<Product>();

            foreach (var fluxRecord in fluxTable.Records)
            {
                var instant = fluxRecord.GetTime();
                var dateTime = instant?.ToDateTimeUtc() ?? DateTime.MinValue;

                var productSalesData = new Product
                {
                    Product_Name = fluxRecord.GetValueByKey("product_name") as string,
                    Units_Sold = Convert.ToInt32(fluxRecord.GetValueByKey("_value")),
                    Time = dateTime,
                };

                productSalesDataList.Add(productSalesData);
            }

            return productSalesDataList;
        }

        public static string GetTimeRangeFilter(DateTime? startTime, DateTime? endTime)
        {
            if (startTime.HasValue && endTime.HasValue)
            {
                return $"|> range(start: {startTime.Value:yyyy-MM-ddTHH:mm:ss}Z, stop: {endTime.Value:yyyy-MM-ddTHH:mm:ss}Z)";
            }
            else
            {
                return "|> range(start: 0)";
            }
        }


        public static DateTime? ParseTime(string timeString)
        {
            if (string.IsNullOrEmpty(timeString))
                return null;

            if (timeString.Equals("now", StringComparison.OrdinalIgnoreCase))
                return DateTime.UtcNow;

            if (DateTime.TryParse(timeString, out var normalDateTime))
                return normalDateTime;

            if (timeString.StartsWith('-') && char.IsDigit(timeString[1]))
            {
                if (timeString.EndsWith("h") && int.TryParse(timeString.Substring(1, timeString.Length - 2), out var hours))
                    return DateTime.UtcNow.AddHours(-hours);

                if (timeString.EndsWith("d") && int.TryParse(timeString.Substring(1, timeString.Length - 2), out var days))
                    return DateTime.UtcNow.AddDays(-days);

                if (timeString.EndsWith("m") && int.TryParse(timeString.Substring(1, timeString.Length - 2), out var minutes))
                    return DateTime.UtcNow.AddMinutes(-minutes);
            }

            // It can be extended for other time units like seconds, etc.

            throw new ArgumentException($"Invalid time format: {timeString}");
        }

    }

}