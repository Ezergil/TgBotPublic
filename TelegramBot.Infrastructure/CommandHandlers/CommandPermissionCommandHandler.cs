using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.Database.Entities;
using TelegramBot.Infrastructure.Services;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TelegramBot.Infrastructure.CommandHandlers
{
    public class CommandPermissionCommandHandler : CommandHandler
    {
        public override string[] PossibleCommands => new[] {"/permissions"};
        public override string Usage => string.Empty;
        private readonly IRepository<CommandPermission> _permissionsRepository;
        private readonly IRepository<User> _usersRepository;
        private readonly CommandHandlerCache _cache;
        private long _userId;
        private string _commandHandlerName;
        protected override bool Public => false;

        public CommandPermissionCommandHandler(ITelegramBotClientAdapter client,
            IRepository<CommandPermission> permissionsRepository,
            CommandHandlerCache cache, IRepository<User> usersRepository) : base(client)
        {
            _permissionsRepository = permissionsRepository;
            _cache = cache;
            _usersRepository = usersRepository;
        }

        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            var chatMember = await Client.GetChatMemberAsync(message.Chat.Id, (int) _userId).ConfigureAwait(false);
            if (chatMember.Status == ChatMemberStatus.Creator)
                throw new Exception("Не трожь батьку!");
            await _permissionsRepository.AddOrUpdateAsync(new CommandPermission
                {
                    ChatId = message.Chat.Id, CommandHandlerName = _commandHandlerName,
                    UserId = _userId, HasAccess = args[3] == "1"
                }, p =>
                    p.ChatId == message.Chat.Id && p.UserId == _userId && p.CommandHandlerName == _commandHandlerName)
                .ConfigureAwait(false);
        }

        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            var user = _usersRepository.SingleOrDefault(u =>
                u.UserName.ToLower().Equals(args[1].Replace("@", String.Empty).ToLower()));
            if (user == null)
                return false;
            _userId = user.Id;
            _commandHandlerName = _cache.GetCommandHandler(args[2])?.GetType().Name;
            if (_commandHandlerName == null)
                return false;
            return (args.Count == 4 && new[]{"0", "1"}.Contains(args[3]));
        }
    }
}