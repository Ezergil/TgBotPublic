using TgBot.Base.Enums;

namespace TgBot.Base.Helpers
{
    public class PeriodTypeConverter
    {
        public static PeriodType FromStringValue(string periodType)
        {
            return periodType.ToLower() == "hour" || periodType.ToLower() == "h" ? PeriodType.Hour :
                periodType.ToLower() == "second" || periodType.ToLower() == "s" ? PeriodType.Second :
                periodType.ToLower() == "minute" || periodType.ToLower() == "m" ? PeriodType.Minute :
                PeriodType.Never;
        }
    }
}
