using System;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using TgBot.Base.Enums;
using TgBot.Base.Helpers;
using TgBot.Services;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.MessageHandlers
{
    public sealed class DeletePolicyMessageHandler : AlwaysEnabledMessageHandler<TelegramMessage>
    {
        private readonly DeleteMessageService _messageService;
        private readonly DeletePolicyService _policyService;

        public DeletePolicyMessageHandler(DeletePolicyService policyService,
            DeleteMessageService messageService, ITelegramBotClientAdapter client) : base((ITelegramBotClientAdapter) client)
        {
            _policyService = policyService;
            _messageService = messageService;
        }

        public override bool Enabled => true;

        protected override async Task HandleMessage(TelegramMessage message)
        {
            var userId = message.From.Id;
            var chatId = message.Chat.Id;
            var messageType = MessageTypeConverter.FromTelegramMessageType(message.Type);

            var existingPolicy = _policyService.GetPolicySettings(chatId, userId, messageType) ??
                _policyService.GetGlobalPolicySettings(chatId, messageType);
            if (existingPolicy.Period != PeriodType.Never)
            {
                await _messageService.AddMessageAsync(new MessageEntry()
                {
                    UserId = userId,
                    ChatId = chatId,
                    MessageType = messageType,
                    MessageId = message.MessageId,
                    DateCreated = DateTime.UtcNow,
                    LiveUntilUtc = LiveUntilDateConverter.GetLiveUntilDate(message.Date, existingPolicy)
                });
            }
        }
    }
}