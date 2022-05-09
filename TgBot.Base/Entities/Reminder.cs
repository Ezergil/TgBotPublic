using System;
using TelegramBot.Infrastructure.Database.Entities;
using TgBot.Base.Enums;

namespace TgBot.Base.Entities
{
    public class Reminder : Entity
    {
        public long ChatId { get; set; }
        public long CreatorId { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public ReminderPeriodType Periodicity { get; set; }

        public bool ShouldNotify(DateTime date)
        {
            switch (Periodicity)
            {
                case ReminderPeriodType.Once : 
                    return date.Subtract(StartTime) <= TimeSpan.FromMinutes(1) && 
                        date.Subtract(StartTime) >= TimeSpan.Zero;
                case ReminderPeriodType.Everyday :
                    return JustPassed(date);
                case ReminderPeriodType.Monthly :
                    return date.Day == StartTime.Day && JustPassed(date);
                case ReminderPeriodType.Weekday :
                    return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday &&
                           JustPassed(date);
                case ReminderPeriodType.Weekend :
                    return (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) &&
                           JustPassed(date);
                case ReminderPeriodType.Yearly :
                    return date.Month == StartTime.Month && date.Day == StartTime.Day && 
                           JustPassed(date);
                default :
                    return false;
            }
        }

        private bool JustPassed(DateTime date)
        {
            return date.TimeOfDay.Subtract(StartTime.TimeOfDay).Duration() <= TimeSpan.FromMinutes(1) && 
                   date.TimeOfDay >= StartTime.TimeOfDay;
        }
    }
}