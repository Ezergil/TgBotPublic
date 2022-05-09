using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TgBot.Base.Interfaces;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class ActivePollsCommandHandler : CommandHandler
    {
        private readonly IPollService _pollService;

        public ActivePollsCommandHandler(ITelegramBotClientAdapter client,
            IPollService pollService) : base(client)
        {
            _pollService = pollService;
        }

        public override string[] PossibleCommands => new[] {"/activepolls", "бот, опросы"};
        public override string Usage => string.Empty;
        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            var activePollMessageIds = _pollService.GetActivePolls(message.Chat.Id).
                Select(p => p.MessageId).ToList();
            if (!activePollMessageIds.Any())
                return;
            var chatId = message.Chat.Id.ToString();
            chatId = chatId.Substring(4, chatId.Length - 4);
            var activePollMessageLinks = string.Join("\r\n", activePollMessageIds.Select(id =>
                $"https://t.me/c/{chatId}/{id}"));
            await Client.SendTextMessageAsync(message.Chat.Id, activePollMessageLinks);
        }
    }
}
