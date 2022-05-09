using System.Linq;
using Telegram.Bot.Types;
using TelegramBot.Infrastructure.Helpers;
using TelegramBot.Infrastructure.Base;

namespace TelegramBot.Infrastructure.DTO
{
    public class TelegramPoll : TelegramUpdate
    {
        public string Id { get; set; }
        public string Question { get; set; }
        public PollOption[] Options { get; set; }
        public string OptionsString => string.Join(";", Options.Select(o => o.Text));

        public override bool Initialize(Update update)
        {
            return this.CopyPropertiesFrom(update.Poll);
        }
    }
}
