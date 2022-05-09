using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TgBot.MessageHandlers;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class ArtemCommandHandler : CommandHandler
    {
        private readonly ArtemMessageHandler _handler;
        public override string[] PossibleCommands => new[] { "/artem", "/artem@ppl_inviter_bot" };
        public override string Usage => string.Empty;
        protected override bool Public => false;

        public ArtemCommandHandler(ArtemMessageHandler handler,
            ITelegramBotClientAdapter client) : base(client)
        {
            _handler = handler;
        }

        protected override Task HandleCommand(TelegramMessage message, List<string> args)
        {
            var toggle = args.Count == 2 ? args[1] == "on" ? true : args[1] == "off" ? false : (bool?) null : null;
            if (toggle.HasValue)
                _handler.Enabled = toggle.Value;
            return Task.CompletedTask;
        }

        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            return args.Count == 2 && new[] {"on", "off"}.Contains(args[1]);
        }
    }
}
