namespace FTOptixNetPlugin.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 获取这个日期的，星期的第一天，周日是星期的第一天
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetWeekFirstDaySun(this DateTime datetime)
        {
            int num = Convert.ToInt32(datetime.DayOfWeek);
            int num2 = -1 * num;
            string value = datetime.AddDays(num2).ToString("yyyy-MM-dd");
            return Convert.ToDateTime(value);
        }
        /// <summary>
        /// 获取这个日期的，星期的第一天，周一是星期的第一天
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetWeekFirstDayMon(this DateTime datetime)
        {
            int num = Convert.ToInt32(datetime.DayOfWeek);
            num = ((num == 0) ? 6 : (num - 1));
            int num2 = -1 * num;
            string value = datetime.AddDays(num2).ToString("yyyy-MM-dd");
            return Convert.ToDateTime(value);
        }
        /// <summary>
        /// 获取这个日期的，星期的最后一天，周六是星期的最后一天
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetWeekLastDaySat(this DateTime datetime)
        {
            int num = Convert.ToInt32(datetime.DayOfWeek);
            int num2 = 7 - num - 1;
            string value = datetime.AddDays(num2).ToString("yyyy-MM-dd");
            return Convert.ToDateTime(value);
        }
        /// <summary>
        /// 获取这个日期的，星期的最后一天，周日是星期的最后一天
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetWeekLastDaySun(this DateTime datetime)
        {
            int num = Convert.ToInt32(datetime.DayOfWeek);
            num = ((num == 0) ? 7 : num);
            int num2 = 7 - num;
            string value = datetime.AddDays(num2).ToString("yyyy-MM-dd");
            return Convert.ToDateTime(value);
        }

        /// <summary>
        /// 获取这个日期的所在月的最后一天
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetMonthLastDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// 获取这个日期的所在月的第一天
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static DateTime GetMonthFirstDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
        }



        public static long ToUnixTimeMilliSeconds(DateTime dateTime)
        {
            DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
            return dto.ToUnixTimeMilliseconds();
        }


        public static long ToUnixTimeSeconds(DateTime dateTime)
        {
            DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
            return dto.ToUnixTimeSeconds();
        }

      

        public static DateTime UTC2Datetime(long ts,int time_unit= 1)
        {
            
            DateTime ttss = DateTime.MinValue;

            switch (time_unit) {
                case 1: 
                    return DateTime.UnixEpoch.AddSeconds(ts);
                case 3:
                    
                    return DateTime.UnixEpoch.AddMilliseconds(ts);
                case 6:
                    return DateTime.UnixEpoch.AddMicroseconds(ts);
                    

            }
            return ttss;

            
        }
    }
}
