using TelegramBot.Infrastructure.Database.Entities;

namespace TgBot.Base.Entities
{
    public class MentionGroupUser : Entity
    {
        public int MentionGroupId { get; set; }
        public long UserId { get; set; }
    }
}