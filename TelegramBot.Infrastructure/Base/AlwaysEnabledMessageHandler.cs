using TelegramBot.Infrastructure.Interfaces;

namespace TelegramBot.Infrastructure.Base
{
    public abstract class AlwaysEnabledMessageHandler<TMessageType> : MessageHandler<TMessageType> where TMessageType : TelegramUpdate, new()
    {
        protected AlwaysEnabledMessageHandler(ITelegramBotClientAdapter client) : base(client)
        {
        }

        public override bool Enabled => true;
    }
}
