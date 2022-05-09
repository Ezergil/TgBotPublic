using System;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using TgBot.Base.Helpers;
using TgBot.Base.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;
using PollAnswer = TgBot.Base.Entities.PollAnswer;

namespace TgBot.MessageHandlers
{
    class PollReminderMessageHandler : AlwaysEnabledMessageHandler<TelegramMessage>
    {
        private readonly IPollService _service;
        private readonly IRepository<ChatUser> _chatUserRepository;

        public PollReminderMessageHandler(ITelegramBotClientAdapter client,
            IPollService service, IRepository<ChatUser> chatUserRepository) : base(client)
        {
            _service = service;
            _chatUserRepository = chatUserRepository;
        }

        protected override async Task HandleMessage(TelegramMessage message)
        {
            var userId = message.From.Id;
            var chatId = message.Chat.Id;
            var chatUser = _chatUserRepository.SingleOrDefault(cu => cu.ChatId == chatId &&
                                                                    cu.UserId == userId);
            var activePoll = _service.GetUnansweredPoll(message.Chat.Id, userId);
            if (activePoll == null)
                return;
            if (chatUser.DateAdded > activePoll.Date && activePoll.IsVote)
                return;
            var answer = _service.GetUserAnswer(userId, activePoll.Id) ??
                         new PollAnswer { Option = -1, PollId = activePoll.Id, UserId = userId };
            var timePassed = DateTime.UtcNow.Subtract(activePoll.Date).TotalHours;
            if (timePassed > 1 && answer.ShouldNotify)
            {
                Message forwardedMessage = null;
                if (chatUser.DateAdded > activePoll.Date)
                    forwardedMessage = await Client.ForwardMessageAsync(chatId, chatId, activePoll.MessageId);
                await Client.SendTextMessageAsync(chatId,
                    $"#голосуй\r\n{MentionGenerator.Generate(userId, message.From.FirstName)},\r\n" +
                    "Стране нужен твой голос! Проголосуй и получи плюсик в карму.", ParseMode.Markdown,
                    forwardedMessage?.MessageId ?? activePoll.MessageId);
                answer.Notify();
                await _service.AnswerPoll(answer);
            }
        }
    }
}