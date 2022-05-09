using System;
using System.Collections.Generic;
using System.Linq;

namespace TgBot.Base.Helpers
{
    public static class EnumHelpers
    {
        public static IEnumerable<string> ToStringList<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(v => v.ToString());
        }

        public static bool Contains<TEnum>(string value) where TEnum : Enum
        {
            return ToStringList<TEnum>().Contains(value, StringComparer.OrdinalIgnoreCase);
        }
    }
}