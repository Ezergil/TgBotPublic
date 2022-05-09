using System;

namespace TgBot.Base.Helpers
{
    public class MentionGenerator
    {
        public static string Generate(long userId, string displayName, string prefix = null)
        {
            prefix += string.IsNullOrEmpty(prefix) ? String.Empty : " ";
            return $"[{prefix}{displayName}](tg://user?id={userId})";
        }
    }
}