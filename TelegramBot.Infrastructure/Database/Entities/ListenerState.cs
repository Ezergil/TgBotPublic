namespace TelegramBot.Infrastructure.Database.Entities
{
    public class ListenerState : Entity
    {
        public string ListenerType { get; set; }
        public long ChatId { get; set; }
        public bool State { get; set; }
    }
}
