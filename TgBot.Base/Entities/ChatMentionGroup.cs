using System.ComponentModel.DataAnnotations;
using TelegramBot.Infrastructure.Database.Entities;

namespace TgBot.Base.Entities
{
    public class ChatMentionGroup : Entity
    {
        [Key]
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string GroupName { get; set; }
    }
}