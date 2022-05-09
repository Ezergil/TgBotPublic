using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using TgBot.Base.Enums;
using TgBot.Base.Helpers;
using TgBot.Services;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Helpers;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class ScheduleDeleteCommandHandler : CommandHandler
    {
        private readonly DeleteMessageService _service;
        private PeriodType _period;

        public ScheduleDeleteCommandHandler(DeleteMessageService messageService,
            ITelegramBotClientAdapter client) : base(client)
        {
            _service = messageService;
        }

        public override string[] PossibleCommands => new[] { "/scheduledelete", "/scheduledelete@ppl_inviter_bot", "/sd" };

        public override string Usage => "Usage: \r\nCommand /scheduledelete should be used as a reply to message " +
            "you want to be deleted soon. Example: /scheduledelete <period> <value>, where possible period value: hour, minute, second";

        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            if (message.From.Id != message.ReplyToMessage.From.Id)
            {
                if (!await ValidatePermissions(message, false))
                    throw new Exception("Убери лапы!");
            }
            int.TryParse(args[2], out var val);
            var replyMessage = new TelegramMessage();
            replyMessage.CopyPropertiesFrom(message.ReplyToMessage);
            var messagesToAdd = new[] {message, replyMessage};
            foreach (var m in messagesToAdd)
            {
                await _service.AddMessageAsync(new MessageEntry()
                {
                    UserId = m.From.Id,
                    ChatId = m.Chat.Id,
                    MessageType = MessageTypeConverter.FromTelegramMessageType(m.Type),
                    MessageId = m.MessageId,
                    LiveUntilUtc = LiveUntilDateConverter.FromDateAdded(m.Date, _period, val)
                });
            }
        }

        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            _period = PeriodTypeConverter.FromStringValue(args[1]);
            return message.ReplyToMessage != null && args.Count >= 3 && _period != PeriodType.Never && 
                   (message.From.Username == message.ReplyToMessage.From.Username || 
                    ValidatePermissions(message).GetAwaiter().GetResult());
        }
    }
}
