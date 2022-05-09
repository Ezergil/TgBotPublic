using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TgBot.Base.Entities;

namespace TgBot.Base.Interfaces
{
    public interface IPollService
    {
        Task CreatePoll(long chatId, string question, string[] options, 
            bool isVote, long createdById, bool allowsMultipleAnswers = false);
        Task AnswerPoll(PollAnswer answer);
        IEnumerable<Poll> GetActivePolls(long chatId);
        IEnumerable<PollAnswer> GetPollAnswers(string pollId);
        Poll GetUnansweredPoll(long chatId, long userId);
        Poll GetById(string id);
        PollAnswer GetUserAnswer(long userId, string pollId);

        Task StopPoll(long chatId, int messageId);

        Poll GetPoll(long chatId, int messageId);
        Task AddOrUpdate(Poll poll, Expression<Func<Poll, bool>> predicate);
    }
}