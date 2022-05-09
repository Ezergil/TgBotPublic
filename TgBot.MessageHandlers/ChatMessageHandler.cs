using System.Threading.Tasks;
using TgBot.Base.Entities;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.MessageHandlers
{
    class ChatMessageHandler : AlwaysEnabledMessageHandler<TelegramMessage>
    {
        private readonly IRepository<Chat> _repository;

        public ChatMessageHandler(ITelegramBotClientAdapter client,
            IRepository<Chat> repository) : base((ITelegramBotClientAdapter) client)
        {
            _repository = repository;
        }

        protected override async Task HandleMessage(TelegramMessage message)
        {

            await _repository.AddOrUpdateAsync(
                new Chat
                {
                    Id = message.Chat.Id,
                    Name = message.Chat.Title
                }, c => c.Id == message.Chat.Id);
        }
    }
}