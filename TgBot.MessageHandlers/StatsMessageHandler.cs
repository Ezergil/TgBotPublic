using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;
using TelegramBot.Infrastructure.Services;

namespace TgBot.MessageHandlers
{
    public class StatsMessageHandler : AlwaysEnabledMessageHandler<TelegramMessage>
    {
        private readonly CachedRepository<Stats> _repository;

        public StatsMessageHandler(ITelegramBotClientAdapter client,
            CachedRepository<Stats> repository) : base((ITelegramBotClientAdapter) client)
        {
            _repository = repository;
        }

        protected override async Task HandleMessage(TelegramMessage message)
        {
            Expression<Func<Stats, bool>> findFunction = (r) =>
                r.Date.Date == message.Date.Date && r.UserId == message.From.Id &&
                r.ChatId == message.Chat.Id;

            var stats = _repository.SingleOrDefault(findFunction) ??
                new Stats
                {
                    UserId = message.From.Id,
                    ChatId = message.Chat.Id,
                    Date = message.Date.Date,
                    MessageCount = 0
                };
            stats.MessageCount++;
            await _repository.AddOrUpdateAsync(stats, findFunction);
        }
    }
}
