using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;
using TelegramBot.Infrastructure.Services;

namespace TgBot.CommandHandlers
{
    public class HelpCommandHandler : CommandHandler
    {
        private readonly CommandHandlerCache _cache;

        public HelpCommandHandler(CommandHandlerCache cache,
            ITelegramBotClientAdapter client) : base(client)
        {
            _cache = cache;
        }

        public override string[] PossibleCommands => new[] { "/help", "/help@ppl_inviter_bot", "бот, команды" };

        public override string Usage => "Usage: \r\nType /help to get list of command and their usage";

        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            var commandHandlers = _cache.GetAll().Where(h => !string.IsNullOrEmpty(h.Usage));
            var helpMessage = new string(commandHandlers.SelectMany(h =>
            $"Command: {h.PossibleCommands.First()}\r\n" +
            $"{h.Usage}\r\n\r\n").ToArray());
            await Client.SendTextMessageAsync(message.Chat.Id, helpMessage);
        }
    }
}
