using System;

namespace TgReminderBot.Core.Data
{
    public struct TimeSpanRange
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public TimeSpanRange(TimeSpan startTime, TimeSpan endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
            if (startTime >= endTime)
            {
                throw new Exception("Start time must be smaller than end time.");
            }
        }
    }
}