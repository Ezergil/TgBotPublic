using System;
using TgBot.Base.Entities;
using TgBot.Base.Enums;

namespace TgBot.Base.Helpers
{
    public class LiveUntilDateConverter
    {
        public static DateTime GetLiveUntilDate(DateTime sentTime, PolicySettings policy)
        {
            TimeSpan timeSpan = new TimeSpan();
            switch (policy.Period)
            {
                case PeriodType.Hour: timeSpan = TimeSpan.FromHours(policy.PeriodValue);break;
                case PeriodType.Minute: timeSpan = TimeSpan.FromMinutes(policy.PeriodValue); break;
                case PeriodType.Second: timeSpan = TimeSpan.FromSeconds(policy.PeriodValue); break;
            }
            return sentTime.Add(timeSpan);
        }

        public static DateTime FromDateAdded(DateTime sentTime, PeriodType periodType, int periodValue)
        {
            TimeSpan timeSpan = new TimeSpan();
            switch (periodType)
            {
                case PeriodType.Hour: timeSpan = TimeSpan.FromHours(periodValue); break;
                case PeriodType.Minute: timeSpan = TimeSpan.FromMinutes(periodValue); break;
                case PeriodType.Second: timeSpan = TimeSpan.FromSeconds(periodValue); break;
            }
            return sentTime.Add(timeSpan);
        }
    }
}