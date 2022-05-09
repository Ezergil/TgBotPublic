using System;
using System.Collections.Generic;
using System.Linq;
using TelegramBot.Infrastructure.Database.Interfaces;
using TgBot.Base.DTO;
using TgBot.Base.Entities;
using TgBot.Base.Helpers;
using TgBot.Base.Interfaces;

namespace TgBot.Services
{
    public class MentionUsersService : IMentionUsersService
    {
        private readonly IRepository<MentionGroupUser> _mentionGroupUserRepository;
        private readonly IRepository<ChatMentionGroup> _chatMentionGroupRepository;
        private readonly IUserService _userService;

        public MentionUsersService(IRepository<MentionGroupUser> mentionGroupUserRepository, 
            IRepository<ChatMentionGroup> chatMentionGroupRepository,
            IUserService userService)
        {
            _mentionGroupUserRepository = mentionGroupUserRepository;
            _chatMentionGroupRepository = chatMentionGroupRepository;
            _userService = userService;
        }

        public List<List<UserModel>> GetPartitionedIds(long chatId, string groupName, List<long> idsToExclude)
        {
            var groupUserIds = new List<long>();
            if (groupName == "кого-нибудь")
            {
                groupUserIds.Add(GetSomeId(chatId, idsToExclude));
            }
            else
            {
               groupUserIds.AddRange(GetGroupIds(chatId, groupName, idsToExclude));
            }
            return groupUserIds.Select(_userService.GetById).ToList().Split(3).ToList();
        }

        private long GetSomeId(long chatId, List<long> idsToExclude)
        {
            var groupUserIds = _userService.GetChatUsers(chatId)
                .Select(u => u.UserId).Except(idsToExclude).ToArray();
            var randomIndex = new Random().Next(0, groupUserIds.Length);
            return groupUserIds.ElementAt(randomIndex);
        }

        private List<long> GetGroupIds(long chatId, string groupName, List<long> idsToExclude)
        {
            var group = _chatMentionGroupRepository.SingleOrDefault(g =>
                g.GroupName == groupName && g.ChatId == chatId);
            return _mentionGroupUserRepository.Find(user =>
                    user.MentionGroupId == group.Id).Select(u => u.UserId)
                .Except(idsToExclude).ToList();
        }
    }
}