using TelegramBot.Infrastructure.Helpers;

namespace TelegramBot.Infrastructure.Database.Entities
{
    public abstract class Entity
    {
        public void CopyFrom(Entity entity)
        {
            this.CopyPropertiesFrom(entity);
        }
    }
}
