using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using TgBot.Base.Entities;
using TgBot.Base.Helpers;
using TgBot.Base.Interfaces;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.Jobs
{
    public class ReminderJob : IInvocable
    {
        private readonly IRepository<Reminder> _reminderRepository;
        private readonly IRepository<Chat> _chatRepository;
        private readonly ITelegramBotClientAdapter _client;
        private readonly IMentionUsersService _mentionUsersService;
        private IUserService _userService;


        public ReminderJob(IRepository<Reminder> reminderRepository, 
            IRepository<Chat> chatRepository, 
            ITelegramBotClientAdapter client, IMentionUsersService mentionUsersService, 
            IUserService userService)
        {
            _reminderRepository = reminderRepository;
            _chatRepository = chatRepository;
            _client = client;
            _mentionUsersService = mentionUsersService;
            _userService = userService;
        }

        public async Task Invoke()
        {
            foreach (var chat in _chatRepository.GetAll())
            {
                Telegram.Bot.Types.Chat tgChat;
                try
                {
                    tgChat = await _client.GetChatAsync(chat.Id);
                }
                catch (ChatNotFoundException)
                {
                    continue;
                }

                if (tgChat.Type == ChatType.Private)
                    continue;
                if (tgChat.Permissions.CanSendMessages.HasValue &&
                    !tgChat.Permissions.CanSendMessages.Value)
                {
                    _chatRepository.Delete(chat);
                    continue;
                }

                var reminders = _reminderRepository.Find(r => 
                    r.ChatId == tgChat.Id);
                foreach (var reminder in reminders)
                {
                    if (reminder.ShouldNotify(DateTime.UtcNow))
                        if (reminder.Name == "birthday")
                        {
                            var users = _mentionUsersService.GetPartitionedIds(chat.Id, "всех",
                                new List<long> {reminder.CreatorId});
                            var birthdayBoy = FirstNameExtractor.
                                Extract(_userService.GetById(reminder.CreatorId).FullName);
                            var messageText = $"{MentionGenerator.Generate(reminder.CreatorId, birthdayBoy, "🎂")}, " +
                                              "всем чатом поздравляем тебя с днём рождения! \r\n" +
                                              "Расти большой, не будь лапшой!";
                            await _client.SendTextMessageAsync(chat.Id, messageText, ParseMode.Markdown);

                            var gifUrls = new[]
                            {
                                "https://media.giphy.com/media/5tlq0pRndGu8U/giphy.gif",
                                "https://media.giphy.com/media/l0ExdXwZquwHGA9Ms/giphy.gif",
                                "https://media.giphy.com/media/3o7qDE31B2gsTCn98A/giphy.gif",
                                "https://media.giphy.com/media/GXnaqmGcg1CTu/giphy.gif",
                                "https://media.giphy.com/media/yoJC2GnSClbPOkV0eA/giphy.gif",
                                "https://media.giphy.com/media/qX7Q4wxpRVo88/giphy.gif",
                                "https://media.giphy.com/media/l46CkATpdyLwLI7vi/giphy.gif",
                                "https://media.giphy.com/media/xT0BKqk8FSsAgRQ0SY/giphy.gif",
                                "https://media.giphy.com/media/13Cz8dQqj7GWk/giphy.gif",
                                "https://media.giphy.com/media/6g564lKXZo796/giphy.gif",
                                "https://media.giphy.com/media/jxTnOS8Mkv8n6/giphy.gif"
                            };
                            var randomIndex = new Random().Next(0, gifUrls.Length);
                            await _client.SendAnimationAsync(chat.Id, gifUrls.ElementAt(randomIndex));

                            foreach (var list in users)
                            {
                                messageText = $"{birthdayBoy} празднует день рождения!\r\n" + 
                                                  string.Join(", ", list.Select(u => 
                                                      MentionGenerator.Generate(u.Id, FirstNameExtractor.Extract(u.FullName))));
                                await _client.SendTextMessageAsync(chat.Id, messageText, ParseMode.Markdown);
                            }
                        }
                        else
                        {
                            await _client.SendTextMessageAsync(tgChat.Id, 
                                $@"{MentionGenerator.Generate(reminder.CreatorId, 
                                    FirstNameExtractor.Extract(_userService.GetById(reminder.CreatorId).FullName))}, " +
                                $"самое время {reminder.Name}!", ParseMode.Markdown);
                        }
                        
                }
            }
        }
    }
}