using Telegram.Bot.Types;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.Helpers;

namespace TelegramBot.Infrastructure.DTO
{
    public sealed class TelegramPollAnswer : TelegramUpdate
    {
        public string PollId { get; set; }
        public User User { get; set; }
        public int[] OptionIds { get; set; }

        public override bool Initialize(Update update)
        {
            return PropertyCopier.CopyPropertiesFrom(this, update.PollAnswer);
        }
    }
}
