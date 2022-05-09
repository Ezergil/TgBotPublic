using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class WhoisCommandHandler : CommandHandler
    {
        public override string[] PossibleCommands => new[] { "/whois" };
        public override string Usage => string.Empty;
        public WhoisCommandHandler(ITelegramBotClientAdapter client) : base(client)
        {
        }
        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            await Client.SendTextMessageAsync(message.Chat.Id, $"Chat id : {message.Chat.Id}\r\nYour id : {message.From.Id}");
        }
    }
}
