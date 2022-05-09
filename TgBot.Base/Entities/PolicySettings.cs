using TelegramBot.Infrastructure.Database.Entities;
using TgBot.Base.Enums;

namespace TgBot.Base.Entities
{
    public class PolicySettings : Entity
    {
        public long UserId { get; set; }
        public long ChatId { get; set; }
        public MessageType MessageType { get; set; }
        public PeriodType Period { get; set; }
        public int PeriodValue { get; set; }
    }
}
