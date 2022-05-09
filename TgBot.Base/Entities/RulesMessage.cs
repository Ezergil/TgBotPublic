using System;
using System.ComponentModel.DataAnnotations;
using TelegramBot.Infrastructure.Database.Entities;

namespace TgBot.Base.Entities
{
    public class RulesMessage : Entity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        [Key]
        public long ChatId { get; set; }
    }
}
