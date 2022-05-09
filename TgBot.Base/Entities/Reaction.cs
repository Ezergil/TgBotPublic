using System.ComponentModel.DataAnnotations;
using TelegramBot.Infrastructure.Database.Entities;
using TgBot.Base.Enums;

namespace TgBot.Base.Entities
{
    public class Reaction : Entity
    {
        [Key]
        public int Id { get; set; }
        public long FromId { get; set; }
        public long ToId { get; set; }
        public ReactionType ReactionType { get; set; }
    }
}