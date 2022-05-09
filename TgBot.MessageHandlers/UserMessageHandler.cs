using System;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using TgBot.Base.Helpers;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.Database.Entities;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;
using TgBot.Base.DTO;
using TgBot.Base.Interfaces;

namespace TgBot.MessageHandlers
{
    class UserMessageHandler : AlwaysEnabledMessageHandler<TelegramMessage>
    {
        private readonly IRepository<ChatUser> _chatUserRepository;
        private readonly IUserService _userService;

        public override int Priority => -10;

        public UserMessageHandler(ITelegramBotClientAdapter client,
            IRepository<ChatUser> chatUserRepository, 
            IUserService userService) : base(client)
        {
            _chatUserRepository = chatUserRepository;
            _userService = userService;
        }

        protected override async Task HandleMessage(TelegramMessage message)
        {
            if (!message.From.IsBot)
            {
                await HandleNewUser(message);
                await HandleNewChatUser(message);
            }
        }

        private async Task HandleNewUser(TelegramMessage message)
        {
            await _userService.AddOrUpdateAsync(
                new UserModel
                {
                    Id = message.From.Id,
                    UserName = message.From.Username,
                    FullName = $"{message.From.FirstName} {message.From.LastName}"
                });
        }

        private async Task HandleNewChatUser(TelegramMessage message)
        {
            var chatId = message.Chat.Id;
            var userId = message.From.Id;
            var chatUser = _chatUserRepository.SingleOrDefault(u 
                => u.UserId == userId && u.ChatId == chatId);
            var date = chatUser?.DateAdded ?? DateTime.UtcNow;
            var chatMember = await Client.GetChatMemberAsync(chatId, userId);
            await _chatUserRepository.AddOrUpdateAsync(
                new ChatUser
                {
                    UserId = userId,
                    ChatId = chatId,
                    DateAdded = date,
                    Role = chatMember.Status.ToUserRole()
                }, u => u.UserId == userId && u.ChatId == chatId);
        }
    }
}