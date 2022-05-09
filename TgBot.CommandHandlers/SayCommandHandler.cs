using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class SayCommandHandler : CommandHandler
    {
        public SayCommandHandler(ITelegramBotClientAdapter client) : base(client)
        {
        }

        public override string[] PossibleCommands => new[] { "/say" };
        public override string Usage => string.Empty;
        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            int messageId = 0;
            if (args.Count == 4)
                int.TryParse(args[3], out messageId);
            if (long.TryParse(args[1], out var chatId))
                await Client.SendTextMessageAsync(chatId, args[2], replyToMessageId: messageId);
        }
    }
}