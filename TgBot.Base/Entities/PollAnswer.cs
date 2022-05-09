using System;
using System.Linq;
using TelegramBot.Infrastructure.Database.Entities;

namespace TgBot.Base.Entities
{
    public class PollAnswer : Entity
    {
        public string PollId { get; set; }
        public long UserId { get; set; }
        public DateTime LastNotifyTime { get; set; }
        public int NotifyCount { get; set; }
        public int Option { get; set; }
        private bool MaxNotifyCountReached => NotifyCount >= 10;
        public bool ShouldNotify => !MaxNotifyCountReached && DateTime.UtcNow
            .Subtract(LastNotifyTime).TotalHours >= 1 && Option == -1;
        public void Notify()
        {
            LastNotifyTime = DateTime.UtcNow;
            NotifyCount++;
        }
        public void Answer(int[] optionIds)
        {
            if (optionIds.Any())
            {
                NotifyCount = 0;
                LastNotifyTime = DateTime.UtcNow;
                Option = optionIds.First();
            }
            else
            {
                Option = -1;
            }
        }
    }
}
