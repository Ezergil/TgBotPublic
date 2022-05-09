using System.Threading.Tasks;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.Database.Entities;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.MessageHandlers
{
    public sealed class ArtemMessageHandler : DbMessageHandler
    {
        public ArtemMessageHandler(ITelegramBotClientAdapter client,
            IRepository<ListenerState> repository) : base(client, repository)
        {
        }

        protected override Task HandleMessage(TelegramMessage message)
        {
            return Task.FromResult(0);
        }
    }
}
