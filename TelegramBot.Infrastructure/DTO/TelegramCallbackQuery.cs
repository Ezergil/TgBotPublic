using Telegram.Bot.Types;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.Helpers;

namespace TelegramBot.Infrastructure.DTO
{
    public class TelegramCallbackQuery : TelegramUpdate
    {
        public string Data { get; set; }
        public Message Message { get; set; }
        public User From { get; set; }
        
        public override bool Initialize(Update update)
        {
            return this.CopyPropertiesFrom(update.CallbackQuery);
        }
    }
}