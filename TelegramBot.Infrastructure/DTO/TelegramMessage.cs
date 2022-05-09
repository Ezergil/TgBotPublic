using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Infrastructure.Helpers;
using TelegramBot.Infrastructure.Base;

namespace TelegramBot.Infrastructure.DTO
{
    public class TelegramMessage : TelegramUpdate
    {
        public override bool Initialize(Update update)
        {
            return this.CopyPropertiesFrom(update.Message);
        }

        public int MessageId { get; set; }
        public User From { get; set; }
        public Chat Chat { get; set; }
        public Message ReplyToMessage { get; set; }
        public User ForwardFrom { get; set; }
        public DateTime Date { get; set; }
        public MessageType Type { get; set; }
    }
}
