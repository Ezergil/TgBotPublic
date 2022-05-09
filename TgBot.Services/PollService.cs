using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using TgBot.Base.Interfaces;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.Services
{
    public class PollService : IPollService
    {
        private readonly IRepository<Poll> _pollRepository;
        private readonly IRepository<PollAnswer> _answerRepository;
        private readonly ITelegramBotClientAdapter _client;

        public PollService(IRepository<Poll> pollRepository,
            IRepository<PollAnswer> answerRepository, ITelegramBotClientAdapter client)
        {
            _pollRepository = pollRepository;
            _answerRepository = answerRepository;
            _client = client;
        }

        public async Task CreatePoll(long chatId, string question, string[] options, bool isVote, long createdById, bool allowsMultipleAnswers = false)
        {
            var response = await _client.SendPollAsync(new Telegram.Bot.Types.ChatId(chatId), question, 
                options, false, allowsMultipleAnswers);
            await _pollRepository.AddOrUpdateAsync(new Poll { ChatId = chatId, Date = response.Date, 
                MessageId = response.MessageId, Id = response.Poll.Id, IsVote = isVote, CreatedById = createdById});
        }

        public async Task AnswerPoll(PollAnswer answer)
        {
            await _answerRepository.AddOrUpdateAsync(answer, 
                a => a.PollId == answer.PollId && a.UserId == answer.UserId);
        }

        public IEnumerable<Poll> GetActivePolls(long chatId)
        {
            return _pollRepository.Find(p => p.ChatId == chatId && p.State != PollState.Stopped);
        }

        public IEnumerable<PollAnswer> GetPollAnswers(string pollId)
        {
            return _answerRepository.Find(a => a.PollId == pollId && a.Option >= 0);
        }

        public Poll GetUnansweredPoll(long chatId, long userId)
        {
            var chatPolls = GetActivePolls(chatId).ToList();
            var chatPollIds = chatPolls.Select(p => p.Id).ToList();
            var answers = _answerRepository.Find(a => a.UserId == userId &&
                                                      chatPollIds.Contains(a.PollId)).ToList();
            var answeredPollIds = answers.Where(a => !a.ShouldNotify).Select(a => a.PollId);
            return chatPolls.FirstOrDefault(p => !answeredPollIds.Contains(p.Id));
        }

        public Poll GetPoll(long chatId, int messageId)
        {
            return _pollRepository.SingleOrDefault(p => p.MessageId == messageId && p.ChatId == chatId);
        }

        public Task AddOrUpdate(Poll poll, Expression<Func<Poll, bool>> predicate)
        {
            return _pollRepository.AddOrUpdateAsync(poll, predicate);
        }

        public Poll GetById(string id)
        {
            return _pollRepository.SingleOrDefault(p => p.Id == id);
        }

        public PollAnswer GetUserAnswer(long userId, string pollId)
        {
            return _answerRepository.SingleOrDefault(a => a.UserId == userId && a.PollId == pollId);
        }

        public async Task StopPoll(long chatId, 
            int messageId)
        {
            var poll = _pollRepository.SingleOrDefault(p => p.MessageId == messageId);
            if (poll.State != PollState.Stopped)
            {
                poll.Stop();
                await _pollRepository.AddOrUpdateAsync(poll, p => p.Id == poll.Id);
                await _client.StopPollAsync(chatId, messageId);
            }
        }
    }
}
