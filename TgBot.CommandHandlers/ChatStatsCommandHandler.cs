using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using TgBot.Base.Helpers;
using TgBot.Base.Interfaces;
using Telegram.Bot.Types.Enums;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;
using TelegramBot.Infrastructure.Services;

namespace TgBot.CommandHandlers
{
    public class ChatStatsCommandHandler : CommandHandler
    {
        private readonly CachedRepository<Stats> _repository;
        private readonly IUserService _userService;

        public override string[] PossibleCommands => new[] { "/chatstats", "/cs", "бот, статистика", "бот статистика" };
        public override string Usage => "Usage: \r\nCommand /chatstats is used to get message count of all chat members";
        private DateTime _date;
        public ChatStatsCommandHandler(CachedRepository<Stats> repository,
            ITelegramBotClientAdapter client, IUserService userService) : base(client)
        {
            _repository = repository;
            _userService = userService;
        }
        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            var messageText = GetDateStats(message, _date, args);
            await Client.SendTextMessageAsync(message.Chat.Id, messageText, parseMode: ParseMode.Html);
        }

        private string GetDateStats(TelegramMessage message, DateTime date, List<string> args)
        {
            var stats = _repository.Find(s => s.ChatId == message.Chat.Id);
            stats = args[1] != "all" ? 
                stats.Where(s => s.Date.Date == date) : 
                stats.GroupBy(s => s.UserId).Select(g => new Stats
                {
                    UserId = g.Key, MessageCount = g.Sum(s => s.MessageCount)
                });
            var statsList = stats.OrderByDescending(s => s.MessageCount).ToList();
            

            var todayIds = statsList.Select(s => s.UserId);
            var users = _userService.GetAll().Where(u => todayIds.Contains(u.Id)).ToList();
            var longestName = users.Select(u => u.FullName.Split(' ').First().Length).Max() + 2;
            var periodString = args[1] == "all" ? "всё время" : date.ToString("dd.MM.yyyy");
            var messageText = new StringBuilder($"Статистика за {periodString}:\r\n\r\n");
            messageText.Append($"Всего сообщений за период: {statsList.Sum(s => s.MessageCount)}\r\n\r\n");
            foreach (var stat in statsList)
            {
                var user = users.FirstOrDefault(u => u.Id == stat.UserId);
                if (user == null)
                    continue;
                messageText.Append($"<pre>|{FirstNameExtractor.Extract(user.FullName).PadRight(longestName)}" +
                                   $"|{stat.MessageCount,-4}|</pre>\r\n");
            }

            return messageText.ToString();
        }

        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            if (args.Count == 1)
                args.Add(DateTime.UtcNow.ToString("dd.MM.yyyy"));
            return args.Count == 1 || args.Count == 2 && (args[1].Equals("all") || DateTime.TryParseExact(args[1],
                "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _date));
        }
    }
}
