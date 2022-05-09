using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TelegramBot.Infrastructure.Database.Entities;

namespace TgBot.Base.Entities
{
   public class UserDetails : Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long UserId { get; set; }
        public string FullName { get; set; }
        public string Prefix { get; set; }
    }
}