using System.ComponentModel.DataAnnotations;
using TelegramBot.Infrastructure.Database.Entities;

namespace TgBot.Base.Entities
{
    public class UserWarnings : Entity
    {
        [Key]
        public long UserId { get; set; }
        public string UserName { get; set; }
        public int Count { get; set; }
    }
}
