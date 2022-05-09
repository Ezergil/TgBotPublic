using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TgBot.Base.Interfaces;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class NewPollCommandHandler : CommandHandler
    {
        private readonly IPollService _service;

        public override string[] PossibleCommands => new[] { "/newpoll" };

        public override string Usage => string.Empty;
        private long _chatId;
        public NewPollCommandHandler(IPollService service, 
            ITelegramBotClientAdapter client): base(client)
        {
            _service = service;
        }

        protected override async Task HandleCommand(TelegramMessage message,
            List<string> args)
        {
            var multiple = args[3] == "mult";
            var argsToSkip = multiple ? 4 : 3;
            var options = args.Skip(argsToSkip).ToArray();
            await _service.CreatePoll(_chatId, args[2], options, false, message.From.Id, multiple);
        }

        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            return args.Count >= 5 && long.TryParse(args[1], out _chatId);
        }
    }
}
