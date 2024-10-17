namespace Ebao.V2.DPEM.Helpers.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ConvertToTimeZone(this DateTime value, string timeZoneId = "E. South America Standard Time")
    {
        if (timeZoneId == TimeZoneInfo.Local.Id)
            return value;

        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            return TimeZoneInfo.ConvertTime(value, TimeZoneInfo.Local, timeZone);
        }
        catch (Exception)
        {
            return value;
        }
    }
}