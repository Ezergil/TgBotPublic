using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;
using TgBot.Base.Interfaces;

namespace TgBot.CommandHandlers
{
    public class SetUserNamePrefixCommandHandler : CommandHandler
    {
        public override string[] PossibleCommands => 
            new []{"/setusernameprefix", "/sunp", "бот, префикс", "бот префикс"};
        public override string Usage => String.Empty;
        private readonly IUserService _userService;
        private string _userName;

        public SetUserNamePrefixCommandHandler(ITelegramBotClientAdapter client,
            IUserService userService) : base(client)
        {
            _userService = userService;
        }

        
        
        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            var user = _userService.GetByUserName(_userName);
            if (args.Count == 4 && new[]{"set", "поставь"}.Contains(args[1]) && user.Id != message.From.Id)
            {
                if (!await ValidatePermissions(message, false))
                    throw new Exception("Убери лапы!");
            }
            if (args.Count == 2)
                await Client.SendTextMessageAsync(message.Chat.Id, user.Prefix);
            else
            {
                var prefix = args[3];
                user.Prefix = prefix;
                await _userService.AddOrUpdateAsync(user);
            }
        }

        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            _userName = args.Count == 2 ? args[1] : args[2];
            return args.Count >= 2;
        }
    }
}