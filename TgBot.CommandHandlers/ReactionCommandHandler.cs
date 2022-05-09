using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using TgBot.Base.Enums;
using TgBot.Base.Helpers;
using Telegram.Bot.Types.Enums;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class ReactionCommandHandler : CommandHandler
    {
        private readonly IRepository<Reaction> _biteRepository;
        public override string[] PossibleCommands => new[] {"кусь", "лизь", "шлёп", "шлеп", "выжигаю глазки"}
            .SelectMany(c => new[] {"", ",", ")", "!"}.Select(s => c + s)).ToArray();
        public override string Usage => string.Empty;
        protected override bool Public => false;
        public ReactionCommandHandler(ITelegramBotClientAdapter client,
            IRepository<Reaction> biteRepository) : base(client)
        {
            _biteRepository = biteRepository;
        }

        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            if (args[0] == "выжигаю глазки")
            {
                await LilithSpecialCase(message);
                return;
            }
            var reactionType = args[0].ToLower().StartsWith("кусь") ? ReactionType.Bite :
                args[0].StartsWith("лизь") ? ReactionType.Lick : ReactionType.Slap;
            var reaction = new Reaction
            {
                FromId = message.From.Id,
                ToId = message.ReplyToMessage.From.Id,
                ReactionType = reactionType
            };
            await _biteRepository.AddOrUpdateAsync(reaction);
            var totalBites = _biteRepository.Find(b => b.FromId == reaction.FromId &&
                                                       b.ToId == reaction.ToId && b.ReactionType ==
                                                       reactionType).Count();
            await Client.SendTextMessageAsync(message.Chat.Id,
                $"{message.From.FirstName} делает " +
                $"{reactionType.ConvertToString()} пользователю " +
                $"{message.ReplyToMessage.From.FirstName}. " +
                $"\r\nИтого это уже {reactionType.ConvertToString()} №{totalBites}", ParseMode.Markdown);
        }

        private async Task LilithSpecialCase(TelegramMessage message)
        {
            if (message.From.Id != 412666607)
                throw new UnauthorizedAccessException();
            await Client.SendTextMessageAsync(message.Chat.Id,
                $"{MentionGenerator.Generate(message.From.Id, "Солнышко", "☀")} " +
                $"изволит выжечь {MentionGenerator.Generate(message.ReplyToMessage.From.Id, "вам")} " +
                $"глазки!", ParseMode.Markdown);
        }

        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            return message.ReplyToMessage != null && message.From != message.ReplyToMessage.From;
        }
    }
}