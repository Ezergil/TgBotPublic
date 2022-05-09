using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class MentionGroupsCommandHandler : CommandHandler
    {
        public override string[] PossibleCommands => new[] {"/mentiongroups", "/mg"};
        public override string Usage => String.Empty;
        private readonly IRepository<ChatMentionGroup> _repository;
        public MentionGroupsCommandHandler(ITelegramBotClientAdapter client, 
            IRepository<ChatMentionGroup> repository) : base(client)
        {
            _repository = repository;
        }



        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            if (args.Count == 1)
            {
                var messageText =
                    string.Join("\r\n",
                        _repository.Find(group => group.ChatId == message.Chat.Id).Select(g => $"{g.GroupName}"));
                await Client.SendTextMessageAsync(message.Chat.Id, messageText);
            }
            else
            {
                if (args.Last() == "add")
                {
                    await _repository.AddOrUpdateAsync(new ChatMentionGroup
                    {
                        ChatId = message.Chat.Id,
                        GroupName = args[1].ToLower()
                    });
                }
                else
                {
                    var chatMentionGroup = _repository.SingleOrDefault(g =>
                        g.ChatId == message.Chat.Id && g.GroupName.Equals(args[1]));
                    _repository.Delete(chatMentionGroup);
                }
            }
        }
        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            return args.Count == 1 || args.Count == 3 && new[] {"add", "delete"}.Contains(args.Last());
        }
    }
}