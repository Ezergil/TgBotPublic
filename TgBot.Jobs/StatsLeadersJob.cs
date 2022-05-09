using System;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using TgBot.Base.DTO;
using TgBot.Base.Entities;
using TgBot.Base.Helpers;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.Interfaces;
using TelegramBot.Infrastructure.Services;
using TgBot.Base.Interfaces;

namespace TgBot.Jobs
{
    public class StatsLeadersJob : IInvocable
    {
        private readonly CachedRepository<Stats> _statsRepository;
        private readonly IRepository<Chat> _chatRepository;
        private readonly IUserService _userService ;
        private readonly ITelegramBotClientAdapter _client;

        public StatsLeadersJob(CachedRepository<Stats> statsRepository,
            IRepository<Chat> chatRepository,
            ITelegramBotClientAdapter client, 
            IUserService userService)
        {
            _statsRepository = statsRepository;
            _chatRepository = chatRepository;
            _userService = userService;
            _client = client;
        }

        public async Task Invoke()
        {
            foreach (var chat in _chatRepository.GetAll())
            {
                try
                {
                    if ((await _client.GetChatAsync(chat.Id)).Type == ChatType.Private) continue;
                }
                catch (ChatNotFoundException)
                {
                    continue;
                }
                var todayStats = _statsRepository.Find(r => r.Date.Date == DateTime.UtcNow.AddDays(-1).Date &&
                                                            r.ChatId == chat.Id).ToList();
                if(!todayStats.Any())
                    continue;
                var top3 = todayStats.OrderByDescending(s => s.MessageCount).Take(3).
                    Select(s => new StatsUserModel{UserId = s.UserId, MessageCount = s.MessageCount}).ToList();
                foreach (var userModel in top3)
                {
                    var user = _userService.GetById(userModel.UserId);
                    userModel.FullName = user.FullName;
                    userModel.Prefix = user.Prefix;
                }
                var messageText = "Итак, сегодняшние лидеры по количеству сообщений : \r\n" +
                                  string.Join("\r\n", top3.Select(u => $"{MentionGenerator.Generate(u.UserId, u.FullName, u.Prefix)} ({u.MessageCount})")) +
                                  "\r\nАффтар жжот! Пеши есчё!";
                await _client.SendTextMessageAsync(chat.Id, messageText, ParseMode.Markdown);
            }
        }
    }
}