using System.Linq;
using System.Text.RegularExpressions;

namespace TgBot.Base.Helpers
{
    public class FirstNameExtractor
    {
        public static string Extract(string fullName)
        {
            var name = fullName.Split(' ').First();
            return Regex.Replace(name, @"[^\u0000-\u0FFF]+", string.Empty);
        }
    }
}