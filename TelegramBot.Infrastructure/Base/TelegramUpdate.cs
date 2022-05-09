using Telegram.Bot.Types;

namespace TelegramBot.Infrastructure.Base
{
    public abstract class TelegramUpdate
    {
        public abstract bool Initialize(Update update);
    }
}
