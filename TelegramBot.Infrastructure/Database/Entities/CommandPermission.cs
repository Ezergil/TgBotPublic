using TelegramBot.Infrastructure.Database.Entities;

public class CommandPermission : Entity
{
    public long ChatId { get; set; }
    public string CommandHandlerName { get; set; }
    public long UserId { get; set; }
    public bool HasAccess { get; set; }
}