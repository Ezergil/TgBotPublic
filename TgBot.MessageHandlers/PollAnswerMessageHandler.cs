using System.Threading.Tasks;
using TgBot.Base.Entities;
using TgBot.Base.Interfaces;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.MessageHandlers
{
    class PollAnswerMessageHandler : AlwaysEnabledMessageHandler<TelegramPollAnswer>
    {
        private readonly IPollService _service;

        public PollAnswerMessageHandler(ITelegramBotClientAdapter client,
            IPollService service) : base(client)
        {
            _service = service;
        }

        protected override async Task HandleMessage(TelegramPollAnswer message)
        {
            var userId = message.User.Id;
            var activePoll = _service.GetById(message.PollId);
            if (activePoll == null)
                return; 
            
            var answer = _service.GetUserAnswer(userId, activePoll.Id) ??
                new PollAnswer { PollId = activePoll.Id, UserId = userId };
            answer.Answer(message.OptionIds);
            await _service.AnswerPoll(answer);
        }
    }
}
