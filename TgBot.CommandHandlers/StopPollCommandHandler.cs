using System.Collections.Generic;
using System.Threading.Tasks;
using TgBot.Base.Interfaces;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class StopPollCommandHandler : CommandHandler
    {
        private readonly IPollService _pollService;

        public StopPollCommandHandler(ITelegramBotClientAdapter client, 
            IPollService pollService) : base(client)
        {
            _pollService = pollService;
        }

        public override string[] PossibleCommands => new[] {"/stoppoll", "/sp"};
        public override string Usage => string.Empty;
        protected override bool Public => false;

        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            await _pollService.StopPoll(message.Chat.Id, message.ReplyToMessage.MessageId);
        }

        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            return (message.ReplyToMessage != null && message.ReplyToMessage.From.IsBot);
        }
    }
}
