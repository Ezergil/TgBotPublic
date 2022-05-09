using System.Linq;

namespace TelegramBot.Infrastructure.Helpers
{
    public static class PropertyCopier
    {
        public static bool CopyPropertiesFrom<T, TU>(this T dest, TU source)
        {
            if (source == null)
                return false;
            var sourceProps = source.GetType().GetProperties().Where(x => x.CanRead).ToList();
            var destProps = dest.GetType().GetProperties()
                    .Where(x => x.CanWrite)
                    .ToList();
            foreach (var sourceProp in sourceProps)
            {
                if (destProps.All(x => x.Name != sourceProp.Name)) continue;
                {
                    var p = destProps.First(x => x.Name == sourceProp.Name);
                    if (p.CanWrite)
                    {
                        p.SetValue(dest, sourceProp.GetValue(source, null), null);
                    }
                }
            }
            return true;
        }
    }
}
