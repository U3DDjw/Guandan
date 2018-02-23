namespace Haoyun.Utils
{
    using UnityEngine;
    using System.Collections;
    using System;


    public class TimeUtils
    {
        public static long BinaryStamp()
        {
            return DateTime.UtcNow.ToBinary();
        }

        /// <summary>
        /// 时间戳转换成日期（当前时区）
        /// </summary>
        /// <returns>The from unix timestamp.</returns>
        /// <param name="unixTimestamp">Unix timestamp.</param>
        /// <param name="kind">Kind.</param>
        public static DateTime ConvertToDate(long unixTimeStamp)
        {
            DateTime unixYear0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = unixTimeStamp * TimeSpan.TicksPerSecond;
            DateTime now = new DateTime(unixYear0.Ticks + unixTimeStampInTicks);
            TimeZone localZone = TimeZone.CurrentTimeZone;
            TimeSpan currentOffset = localZone.GetUtcOffset(now);
            DateTime targetTime = now.Add(currentOffset);
            return targetTime;
        }

        /// <summary>
        /// 时间戳Ticks转换成日期（当前时区）
        /// </summary>
        /// <returns>The from unix timestamp in ticks.</returns>
        /// <param name="unixTimeStampInTicks">Unix time stamp in ticks.</param>
        /// <param name="kind">Kind.</param>
        public static DateTime ConvertToDataInTicks(long unixTimeStampInTicks)
        {
            DateTime unixYear0 = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            DateTime now = new DateTime(unixYear0.Ticks + unixTimeStampInTicks);
            TimeZone localZone = TimeZone.CurrentTimeZone;
            TimeSpan currentOffset = localZone.GetUtcOffset(now);
            DateTime targetTime = now.Add(currentOffset);
            return targetTime;
        }

        /// <summary>
        /// 日期转换成时间戳（当前时区）
        /// </summary>
        /// <returns>The timestamp from date time.</returns>
        /// <param name="date">Date.</param>
        public static long ConvertToTime(DateTime time)
        {
            //long unixTimestamp = date.Ticks - new DateTime (1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
            //unixTimestamp /= TimeSpan.TicksPerSecond;
            //return unixTimestamp;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (long)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// 当前时间戳 毫秒数
        /// </summary>
        /// <returns></returns>
        public static long CurMilliseconds()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// 把时间秒数转化成字符串00:00:00
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string ConvertToHMS(long second)
        {
            string str = "";
            long hour = second / 3600;
            long min = second % 3600 / 60;
            long sec = second % 60;
            if (hour < 10)
            {
                str += "0" + hour.ToString();
            }
            else
            {
                str += hour.ToString();
            }
            str += ":";
            if (min < 10)
            {
                str += "0" + min.ToString();
            }
            else
            {
                str += min.ToString();
            }
            str += ":";
            if (sec < 10)
            {
                str += "0" + sec.ToString();
            }
            else
            {
                str += sec.ToString();
            }


            return str;
        }
        /// <summary>
        /// 时间的差值
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetDateTimeIntervalSeconds(System.DateTime timeBefore, System.DateTime timeAfter)
        {
            return (int)Mathf.Abs((int)timeBefore.Subtract(timeAfter).TotalSeconds);
        }
        /// <summary>
        /// DateTime时间格式转换为10位不带毫秒的Unix时间戳
        /// </summary>
        /// <param name="time"> DateTime时间格式</param>
        /// <returns>Unix时间戳格式</returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
    }
}