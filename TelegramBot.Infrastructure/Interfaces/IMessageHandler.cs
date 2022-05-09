using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Infrastructure.Interfaces
{
    public interface IMessageHandler
    {
        Task Listen(Update update);
        int Priority { get; }
    }
}