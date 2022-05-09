using TgBot.Base.Enums;

namespace TgBot.Base.Helpers
{
    public class MessageTypeConverter
    {
        public static MessageType FromStringValue(string messageType)
        {
            if (messageType.ToLower() == "media")
                return MessageType.Media;
            return MessageType.Text;
        }

        public static MessageType FromTelegramMessageType(Telegram.Bot.Types.Enums.MessageType telegramMessageType)
        {
            if (telegramMessageType == Telegram.Bot.Types.Enums.MessageType.Text)
                return MessageType.Text;
            return MessageType.Media;
        }
    }
}
