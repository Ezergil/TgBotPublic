using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBot.Base.Helpers;
using TgBot.Services;
using Telegram.Bot.Types.Enums;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.Database.Entities;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;
using TgBot.Base.Interfaces;

namespace TgBot.CommandHandlers
{
    public class ChatPoliciesCommandHandler : CommandHandler
    {
        private readonly DeletePolicyService _service;
        private readonly IUserService _userService;

        public override string[] PossibleCommands => new[] { "/chatpolicies", "/cp" };

        public override string Usage => string.Empty;

        public ChatPoliciesCommandHandler(DeletePolicyService service,
            ITelegramBotClientAdapter client, 
            IUserService userService) : base(client)
        {
            _service = service;
            _userService = userService;
        }

        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            var chatPolicies = _service.GetForChat(message.Chat.Id).
                OrderBy(p => p.UserId).
                ThenBy(p => p.MessageType).ToList();
            var users = _userService.GetAll().ToList();
            var longestName = users.
                Select(u => u.FullName.Split(' ').First().Length).Max() + 2;
            var messageText = new StringBuilder();
            foreach (var settings in chatPolicies)
            {
                var user = users.FirstOrDefault(u => u.Id == settings.UserId);
                if (user == null)
                    continue;
                messageText.Append($"<pre>|{FirstNameExtractor.Extract(user.FullName).PadLeft(longestName)}|{settings.MessageType.ToString(), 5}|" +
                                   $"{settings.Period.ToString()}|{ settings.PeriodValue}|</pre>\r\n");
            }
            await Client.SendTextMessageAsync(message.Chat.Id, messageText.ToString(), ParseMode.Html);
        }
    }
}
