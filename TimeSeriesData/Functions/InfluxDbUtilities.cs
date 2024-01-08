using NodaTime;

namespace Timeseriesdata.Functions
{

    public class InfluxDbUtilities
    {
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
                if (timeString.EndsWith("w") && int.TryParse(timeString.Substring(1, timeString.Length - 2), out var weeks))
                    return DateTime.UtcNow.AddDays(-weeks * 7);

                if (timeString.EndsWith("M") && int.TryParse(timeString.Substring(1, timeString.Length - 2), out var months))
                {
                    return AdjustMonths(DateTime.UtcNow, -months);
                }

                if (timeString.EndsWith("y") && int.TryParse(timeString.Substring(1, timeString.Length - 2), out var years))
                    return DateTime.UtcNow.AddYears(-years);

                if (timeString.EndsWith("h") && int.TryParse(timeString.Substring(1, timeString.Length - 2), out var hours))
                    return DateTime.UtcNow.AddHours(-hours);

                if (timeString.EndsWith("d") && int.TryParse(timeString.Substring(1, timeString.Length - 2), out var days))
                    return DateTime.UtcNow.AddDays(-days);

                if (timeString.EndsWith("m") && int.TryParse(timeString.Substring(1, timeString.Length - 2), out var minutes))
                    return DateTime.UtcNow.AddMinutes(-minutes);

                if (timeString.EndsWith("s") && int.TryParse(timeString.Substring(1, timeString.Length - 2), out var seconds))
                    return DateTime.UtcNow.AddSeconds(-seconds);

                // It can be extended for other time units.
            }

            throw new ArgumentException($"Invalid time format: {timeString}");
        }

        public static DateTime AdjustMonths(DateTime dateTime, int months)
        {
            if (months > 0)
                return dateTime.AddMonths(months);

            int year = dateTime.Year;
            int month = dateTime.Month + months;

            while (month <= 0)
            {
                month += 12;
                year--;
            }

            int daysInMonth = DateTime.DaysInMonth(year, month);
            int day = Math.Min(dateTime.Day, daysInMonth);

            return new DateTime(year, month, day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);
        }

        public static DateTime? ConvertInstantToDateTime(Instant? instant)
         {
                return instant?.InUtc().ToDateTimeUtc();
         }   

        public static long ToInfluxTimestamp(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                var utcDateTime = dateTime.Value.ToUniversalTime();
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var timestamp = (long)(utcDateTime - epoch).TotalSeconds;

                return timestamp;
            }

            return 0;
        }


        }
}