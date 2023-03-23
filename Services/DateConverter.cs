namespace static_sv.Services
{
    public class DateConverter
    {
        public static long ToTimestamp(DateTime dt)
        {
            // DateTime dateTime = DateTime.Now; // or any other DateTime object
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long unixTimestamp = (long)(dt.ToUniversalTime() - unixEpoch).TotalSeconds;
            return unixTimestamp;
        }
    }
}