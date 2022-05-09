using System;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using TgBot.Base.Helpers;
using TgBot.Base.Interfaces;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.Interfaces;
using Chat = TgBot.Base.Entities.Chat;
using Poll = TgBot.Base.Entities.Poll;

namespace TgBot.Jobs
{
    public class PollNotifierJob : IInvocable
    {
        private readonly IPollService _pollService;
        private readonly IRepository<Chat> _chatRepository;
        private readonly ITelegramBotClientAdapter _client;
        private readonly IUserService _userService;

        public PollNotifierJob(IRepository<Chat> chatRepository,
            ITelegramBotClientAdapter client, IPollService pollService, 
            IUserService userService)
        {
            _chatRepository = chatRepository;
            _client = client;
            _pollService = pollService;
            _userService = userService;
        }

        public async Task Invoke()
        {
            foreach (var chat in _chatRepository.GetAll())
            {
                try
                {
                    await ProcessChat(chat);
                }
                catch (Exception e)
                {
                    //suppress
                }
            }
        }

        private async Task ProcessChat(Chat chat)
        {
            Telegram.Bot.Types.Chat tgChat;
            try
            {
                tgChat = await _client.GetChatAsync(chat.Id);
            }
            catch (ChatNotFoundException)
            {
                return;
            }

            if (tgChat.Type == ChatType.Private)
                return;
            if (tgChat.Permissions.CanSendMessages.HasValue &&
                !tgChat.Permissions.CanSendMessages.Value)
            {
                _chatRepository.Delete(chat);
                return;
            }

            var members = _userService.GetChatUsers(chat.Id);
            var membersCount = members.Count();
            var activePolls = _pollService.GetActivePolls(chat.Id);
            foreach (var poll in activePolls)
            {
                await ProcessPoll(chat, poll, membersCount);
            }
        }

        private async Task ProcessPoll(Chat chat, Poll poll, int membersCount)
        {
            var answers = _pollService.GetPollAnswers(poll.Id);
            var quotaReached = answers.Count() / (float) membersCount;
            if (Math.Abs(quotaReached - 1) < 0.01 || quotaReached >= 0.7 && poll.Date < DateTime.UtcNow.AddDays(-1))
            {
                var creator = _userService.GetById(poll.CreatedById);
                try
                {
                    await _client.SendTextMessageAsync(chat.Id,
                        $@"{MentionGenerator.Generate(creator.Id,
                            FirstNameExtractor.Extract(creator.FullName))}!" +
                        "\r\nКворум набран, принимай работу.", ParseMode.Markdown,
                        poll.MessageId);
                }
                catch (Exception e)
                {
                    await _pollService.StopPoll(chat.Id, poll.MessageId);
                }
            }
        }
    }
}