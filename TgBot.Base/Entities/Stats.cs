using System;
using TelegramBot.Infrastructure.Database.Entities;

namespace TgBot.Base.Entities
{
    public class Stats : Entity
    {
        public DateTime Date { get; set; }
        public long UserId { get; set; }
        public long ChatId { get; set; }
        public int MessageCount { get; set; }
    }
}
