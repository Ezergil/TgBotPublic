using System.Threading.Tasks;
using Coravel.Invocable;
using TgBot.Base.Entities;
using Telegram.Bot.Types.Enums;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.Jobs
{
    public class RemoveChatUserJob : IInvocable
    {
        private readonly IRepository<Chat> _chatRepository;
        private readonly IRepository<ChatUser> _chatUserRepository;
        private readonly ITelegramBotClientAdapter _client;
        private readonly IRepository<Reminder> _reminderRepository;

        public RemoveChatUserJob(IRepository<Chat> chatRepository, 
            IRepository<ChatUser> chatUserRepository, ITelegramBotClientAdapter client, 
            IRepository<Reminder> reminderRepository)
        {
            _chatRepository = chatRepository;
            _chatUserRepository = chatUserRepository;
            _client = client;
            _reminderRepository = reminderRepository;
        }

        public async Task Invoke()
        {
            var chats =  _chatRepository.GetAll();
            foreach (var chat in chats)
            {
                var chatUsers = _chatUserRepository.
                    Find(cu => cu.ChatId == chat.Id);
                foreach (var chatUser in chatUsers)
                {
                    var user = await _client.GetChatMemberAsync(chat.Id, (int) chatUser.UserId);
                    if (user.Status == ChatMemberStatus.Kicked || user.Status == ChatMemberStatus.Left)
                    {
                        _chatUserRepository.Delete(chatUser);
                        var reminders = _reminderRepository.Find(r =>
                            r.ChatId == chat.Id && r.CreatorId == chatUser.UserId);
                        foreach (var reminder in reminders)
                        {
                            _reminderRepository.Delete(reminder);
                        }
                    }
                }
            }
        }
    }
}