using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TgBot.Base.Interfaces;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class PollNotificationDeferCommandHandler : CommandHandler
    {
        public override string[] PossibleCommands => new[] {"/defer", "бот, отложи", "бот отложи"};
        public override string Usage => string.Empty;
        private readonly IPollService _pollService;
        public PollNotificationDeferCommandHandler(ITelegramBotClientAdapter client, IPollService pollService) : base(client)
        {
            _pollService = pollService;
        }
        protected override Task HandleCommand(TelegramMessage message, List<string> args)
        {
            var poll = _pollService.GetPoll(message.Chat.Id, message.ReplyToMessage.MessageId);
            poll.Date = DateTime.UtcNow;
            return _pollService.AddOrUpdate(poll, p => p.Id == poll.Id);
        }

        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            return (message.ReplyToMessage != null && message.ReplyToMessage.From.IsBot);
        }
    }
}