using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using TgBot.Base.Enums;
using TgBot.Base.Helpers;
using TgBot.Services;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class DeletePolicyCommandHandler : CommandHandler
    {
        private readonly DeletePolicyService _service;

        public DeletePolicyCommandHandler(DeletePolicyService service,
            ITelegramBotClientAdapter client) : base(client)
        {
            _service = service;
        }

        public override string[] PossibleCommands => new[] { "/deletepolicy", "/deletepolicy@ppl_inviter_bot" };

        public override string Usage => "Usage: \r\nCommand /deletepolicy configures lifetime of messages sent by you." +
            "\r\nExample: /deletepolicy <type> <period> <value>, where possible period value: hour, minute, second" +
            "\r\nAnd possible type values are media (those are all but text messages : stickers, voice, gifs, photos, videos) and text";
        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            var messageType = args[1];
            var periodType = args[2];
            int.TryParse(args.Count == 4 ? args[3] : string.Empty, out var val);

            var settings = new PolicySettings
            {
                MessageType = MessageTypeConverter.FromStringValue(messageType),
                Period = periodType.ToLower() == "hour" || periodType.ToLower() == "h" ? PeriodType.Hour :
                periodType.ToLower() == "second" || periodType.ToLower() == "s" ? PeriodType.Second :
                periodType.ToLower() == "minute" || periodType.ToLower() == "m" ? PeriodType.Minute :
                PeriodType.Never,
                PeriodValue = val,
                ChatId = message.Chat.Id,
                UserId = message.From.Id,
            };
            await Client.SendTextMessageAsync(message.Chat.Id, $"Policy for {message.From.Username} succesfully set. {settings.MessageType.ToString()} {settings.Period.ToString()} " +
                                                                     $"{(settings.PeriodValue == 0 ? string.Empty : settings.PeriodValue.ToString())}");
            await _service.AddOrUpdatePolicySettingsAsync(settings);
        }

        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            return args.Count >= 3 && new[] {"media", "text"}.Contains(args[1],StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
