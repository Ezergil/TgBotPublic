using TgBot.Base.Enums;

namespace TgBot.Base.Helpers
{
    public static class ReactionTypeToStringConverter
    {
        public static string ConvertToString(this ReactionType reactionType)
        {
            switch (reactionType)
            {
                case ReactionType.Bite: return "Кусь";
                case ReactionType.Lick: return "Лизь";
                default: return "Шлёп";
            }
        }
    }
}