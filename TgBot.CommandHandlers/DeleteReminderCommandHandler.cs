using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class DeleteReminderCommandHandler : CommandHandler
    {
        public override string[] PossibleCommands => new[] {"/deletereminder"};
        public override string Usage => String.Empty;
        private readonly IRepository<Reminder> _reminderRepository;
        public DeleteReminderCommandHandler(ITelegramBotClientAdapter client, 
            IRepository<Reminder> reminderRepository) : base(client)
        {
            _reminderRepository = reminderRepository;
        }

        protected override Task HandleCommand(TelegramMessage message, List<string> args)
        {
            var reminder = _reminderRepository.SingleOrDefault(r =>
                r.ChatId == message.Chat.Id && r.CreatorId == message.From.Id &&
                r.Name.ToLower().Equals(args[1].ToLower()));
            if (reminder.Name != "birthday")
                _reminderRepository.Delete(reminder);
            return Task.CompletedTask;
        }
    }
}