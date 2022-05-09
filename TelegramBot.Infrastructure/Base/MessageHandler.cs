using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.Infrastructure.Helpers;
using TelegramBot.Infrastructure.Interfaces;

namespace TelegramBot.Infrastructure.Base
{
    public abstract class MessageHandler<TMessageType> : IMessageHandler where TMessageType : TelegramUpdate, new()
    {
        protected readonly ITelegramBotClientAdapter Client;

        protected MessageHandler(ITelegramBotClientAdapter client)
        {
            Client = client;
            Priority = 0;
        }

        public virtual bool Enabled { get; set; }
        public virtual int Priority { get; }

        protected virtual void Setup(Update update)
        {
            //could be overriden by subclasses
        }

        public async Task Listen(Update update)
        {
            var message = new TMessageType();
            if (!message.Initialize(update))
                return;
            Setup(update);
            if (Enabled)
                await RetryPolicy.AsyncPolicy<Exception>().ExecuteAsync(async () => 
                    await HandleMessage(message).ConfigureAwait(false)).ConfigureAwait(false);
        }

        protected abstract Task HandleMessage(TMessageType message);
    }
}
