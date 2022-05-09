using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;
using TgBot.Base.Interfaces;

namespace TgBot.CommandHandlers
{
    public class MentionGroupUsersCommandHandler : CommandHandler
    {
        public override string[] PossibleCommands => new []{"/mentiongroupusers", "/mgu"};
        public override string Usage => String.Empty;
        private readonly IRepository<MentionGroupUser> _mentionGroupUserRepository;
        private readonly IRepository<ChatMentionGroup> _chatMentionGroupRepository;
        private readonly IUserService _userService;

        public MentionGroupUsersCommandHandler(ITelegramBotClientAdapter client, 
            IRepository<MentionGroupUser> mentionGroupUserRepository, 
            IRepository<ChatMentionGroup> chatMentionGroupRepository, 
            IUserService userService) : base(client)
        {
            _mentionGroupUserRepository = mentionGroupUserRepository;
            _chatMentionGroupRepository = chatMentionGroupRepository;
            _userService = userService;
        }


        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            if (args.Count == 2)
            {
                var group = _chatMentionGroupRepository.SingleOrDefault(g =>
                    g.GroupName == args[1] && g.ChatId == message.Chat.Id);
                var groupUserIds = _mentionGroupUserRepository.Find(user =>
                    user.MentionGroupId == group.Id).Select(u => u.UserId).ToList();
                var users = _userService.GetAll().ToList();
                users = users.Where(u => groupUserIds.Contains(u.Id)).ToList();
                var messageText = string.Join("\r\n", users.Select(u =>
                    u.FullName));
                await Client.SendTextMessageAsync(message.Chat.Id, messageText);
            }
            else
            {
                var group = _chatMentionGroupRepository.SingleOrDefault(g =>
                    g.GroupName == args[1] && g.ChatId == message.Chat.Id);
                var userId = _userService.GetByUserName(args[2]).Id;
                if (args.Last() == "add")
                {
                    await _mentionGroupUserRepository.AddOrUpdateAsync(new MentionGroupUser
                    {
                        MentionGroupId = group.Id,
                        UserId = userId
                    });
                }
                else
                {
                    var mentionGroupUser = _mentionGroupUserRepository.SingleOrDefault(m =>
                        m.UserId == userId && m.MentionGroupId == group.Id);
                    _mentionGroupUserRepository.Delete(mentionGroupUser);
                }
            }
        }
        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            return args.Count == 2 || args.Count == 4 && new[] {"add", "delete"}.Contains(args.Last());
        }
    }
}