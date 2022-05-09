using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using TgBot.Base.Enums;
using TgBot.Base.Helpers;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class RemindersCommandHandler : CommandHandler
    {
        public override string[] PossibleCommands => new[] {"/reminders"};
        public override string Usage => string.Empty;
        private readonly IRepository<Reminder> _remindersRepository;
        private DateTime _date;

        public RemindersCommandHandler(ITelegramBotClientAdapter client,
            IRepository<Reminder> remindersRepository) : base(client)
        {
            _remindersRepository = remindersRepository;
        }
        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            if (args.Count == 1)
                await Client.SendTextMessageAsync(message.Chat.Id,
                   String.Join("\r\n", _remindersRepository.Find(r =>
                        r.ChatId == message.Chat.Id && r.CreatorId == message.From.Id && r.Name != "birthday").
                        Select(r => r.Name)));
            else
            {
                Enum.TryParse<ReminderPeriodType>(args[3], true, out var periodicity);
                var name = args[1].ToLower() == "birthday" ? "birthday" : args[1];
                if (name == "birthday")
                {
                    _date = _date.Date.AddHours(9);
                    periodicity = ReminderPeriodType.Yearly;
                }
                var reminder = new Reminder
                {
                    ChatId = message.Chat.Id,
                    CreatorId = message.From.Id,
                    Name = name,
                    StartTime = _date,
                    Periodicity = periodicity
                };
                await _remindersRepository.AddOrUpdateAsync(reminder, r => 
                    r.ChatId == reminder.ChatId && r.CreatorId == reminder.CreatorId && r.Name == reminder.Name);
            }
        }

        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            return args.Count == 1 || args.Count == 4 &&
                EnumHelpers.Contains<ReminderPeriodType>(args[3]) && DateTime.TryParseExact(args[2],
                    "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, out _date);
        }
    }
}