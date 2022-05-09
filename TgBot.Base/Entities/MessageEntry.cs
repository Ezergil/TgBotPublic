using System;
using TelegramBot.Infrastructure.Database.Entities;
using TgBot.Base.Enums;

namespace TgBot.Base.Entities
{
    public class MessageEntry : Entity
    {
        public MessageType MessageType { get; set; }
        public long ChatId { get; set; }
        public long UserId { get; set; }
        public int MessageId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LiveUntilUtc { get; set; }
    }
}