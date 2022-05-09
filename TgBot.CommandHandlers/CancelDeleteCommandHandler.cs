using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TgBot.Services;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class CancelDeleteCommandHandler : CommandHandler
    {
        private readonly DeleteMessageService _service;
        public override string[] PossibleCommands => new[] { "/canceldelete", "/canceldelete@ppl_inviter_bot", "/cd" };

        public override string Usage => "Usage: \r\nCommand /canceldelete should be used as a reply to message you want not to be deleted";

        public CancelDeleteCommandHandler(ITelegramBotClientAdapter client,
            DeleteMessageService messageService) : base(client)
        {
            _service = messageService;
        }
        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            if (message.From.Id != message.ReplyToMessage.From.Id)
            {
                if (!await ValidatePermissions(message, false))
                    throw new Exception("Убери лапы!");
            }
            _service.RemoveMessage(message.Chat.Id, message.ReplyToMessage.MessageId);
            await Client.SendTextMessageAsync(message.Chat.Id, "Success");
        }

        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            return message.ReplyToMessage != null;
        }
    }
}
