using System;
using System.Collections.Generic;
using System.Text;

namespace TgReminderBot.Data
{
    public class MailingSchedule
    {
        public TimeSpanRange Range { get; set; } = new TimeSpanRange(TimeSpan.FromHours(11), TimeSpan.FromHours(22));

        public TimeSpan Delay { get; set; } = TimeSpan.FromMinutes(60);

        public int TimeZone { get; set; } = 0;

        public MailingSchedule() { }

        /// <summary>
        /// String must be like '11-22, +3, 1', which mean '{StartHour}-{EndHour}, +{TimeZone}, {DelayMinutes}'.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static MailingSchedule FromHoursString(string str)
        {
            try
            {
                if (str == null)
                    throw new ArgumentNullException(nameof(str));
                str = str.Trim();
                var arr = str.Split(",");
                if (arr.Length != 3)
                {
                    throw new Exception();
                }
                var rangeArr = arr[0].Split("-");
                var shedule = new MailingSchedule();
                shedule.Range = new TimeSpanRange(
                    TimeSpan.FromHours(Convert.ToInt32(rangeArr[0].Trim())),
                    TimeSpan.FromHours(Convert.ToInt32(rangeArr[1].Trim()))
                );
                shedule.TimeZone = Convert.ToInt32(arr[1].Replace("+", "").Trim());
                shedule.Delay = TimeSpan.FromMinutes(Convert.ToInt32(arr[2].Trim()));
                return shedule;
            }
            catch (Exception ex)
            {
                throw new Exception("Parse exception.", ex);
            }
        }

        public string ToHoursString()
        {
            return $"{Range.StartTime.TotalHours}-{Range.EndTime.TotalHours}, +{TimeZone}, {Delay.TotalMinutes}";
        }
    }
}
