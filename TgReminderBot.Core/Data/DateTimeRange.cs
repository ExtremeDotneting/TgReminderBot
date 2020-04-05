using System;

namespace TgReminderBot.Core.Data
{
    public struct DateTimeRange
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public DateTimeRange(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
            if (startTime >= endTime)
            {
                throw new Exception("Start time must be smaller than end time.");
            }
        }

        public DateTimeRange(DateTime startTime, TimeSpan addToStart)
            : this(startTime, startTime + addToStart)
        {
        }
    }
}