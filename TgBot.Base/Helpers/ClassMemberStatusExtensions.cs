using Telegram.Bot.Types.Enums;
using TgBot.Base.Enums;

namespace TgBot.Base.Helpers
{
    public static class ClassMemberStatusExtensions
    {
        public static UserRole ToUserRole(this ChatMemberStatus status)
        {
            switch (status)
            {
                case ChatMemberStatus.Administrator: return UserRole.Administrator;
                case ChatMemberStatus.Creator: return UserRole.Creator;
                default: return UserRole.Member;
            }
        }
    }
}