using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;
using TelegramBot.Infrastructure.Services;

namespace TgBot.CommandHandlers
{
    public class MyStatsCommandHandler : CommandHandler
    {
        private readonly CachedRepository<Stats> _repository;

        public override string[] PossibleCommands => new[] { "/mystats" };
        public override string Usage => "Usage: \r\nCommand /mystats is used to get you message count";

        public MyStatsCommandHandler(CachedRepository<Stats> repository,
            ITelegramBotClientAdapter client) : base(client)
        {
            _repository = repository;
        }
        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            var stats = _repository.Find(s => s.ChatId == message.Chat.Id &&
                s.UserId == message.From.Id).ToArray();
            var todayStats = stats.FirstOrDefault(s => s.Date.Date == message.Date.Date);
            if (todayStats != null)
            {
                var messageText = $@"Статистика пользователя {message.From.Username} : 
Сообщений сегодня: {todayStats.MessageCount}
Сообщений всего: {stats.Sum(s => s.MessageCount)}";
                await Client.SendTextMessageAsync(message.Chat.Id, messageText);
            }
        }
    }
}
