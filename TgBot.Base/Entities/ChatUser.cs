using System;
using TelegramBot.Infrastructure.Database.Entities;
using TgBot.Base.Enums;

namespace TgBot.Base.Entities
{
    public class ChatUser : Entity
    {
        public DateTime DateAdded { get; set; }
        public long ChatId { get; set; }
        public long UserId { get; set; }
        public UserRole Role { get; set; }
    }
}
