using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TgBot.Base.Helpers;
using TgBot.Base.Interfaces;
using Telegram.Bot.Types.Enums;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class MentionCommandHandler : CommandHandler
    {
        public override string[] PossibleCommands =>
            new[] {"/mention", "бот, позови", "бот позови"};
        public override string Usage => String.Empty;
        private readonly IMentionUsersService _mentionUsersService;

        public MentionCommandHandler(ITelegramBotClientAdapter client, 
            IMentionUsersService mentionUsersService) : base(client)
        {
            _mentionUsersService = mentionUsersService;
        }


        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            var users = _mentionUsersService.GetPartitionedIds(message.Chat.Id, args[1],
                new List<long> {message.From.Id});
            
            foreach (var list in users)
            {
                var messageText = $"{message.From.FirstName} вызывает {args[1]}!\r\n" + 
                                  string.Join(", ", list.Select(u => 
                                      MentionGenerator.Generate(u.Id, FirstNameExtractor.Extract(u.FullName))));
                await Client.SendTextMessageAsync(message.Chat.Id, messageText, ParseMode.Markdown);
            }
        }

        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            return args.Count >= 2;
        }
    }
}