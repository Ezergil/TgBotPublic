using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class LeaveCommandHandler : CommandHandler
    {
        public LeaveCommandHandler(ITelegramBotClientAdapter client) : base(client)
        {
        }

        public override string[] PossibleCommands => new[] {"/leave"};
        public override string Usage => string.Empty;
        protected override bool Public => false;

        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            await Client.LeaveChatAsync(message.Chat);
        }
    }
}