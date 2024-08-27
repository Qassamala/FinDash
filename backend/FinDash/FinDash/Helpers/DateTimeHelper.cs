using System.Globalization;

namespace FinDash.Helpers
{
    public class DateTimeHelper
    {
        public static string ConvertTimestamp(System.DateTime timestamp)
        {
            return timestamp.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
