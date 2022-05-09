using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.Infrastructure.Interfaces;

namespace TelegramBot.Infrastructure.Services
{
    public sealed class MessageHandlerCache
    {
        private readonly IEnumerable<IMessageHandler> _messageHandlers;

        public MessageHandlerCache(IEnumerable<IMessageHandler> messageHandlers)
        {
            _messageHandlers = messageHandlers;
        }

        public async Task Listen(Update update)
        {
            if (_messageHandlers != null)
            {
                var grouppedHandlers = 
                    _messageHandlers.GroupBy(mh => mh.Priority);
                foreach (var group in grouppedHandlers.OrderBy(g => g.Key))
                {
                    await Task.WhenAll(group.Select(m => m.Listen(update))).ConfigureAwait(false);
                }
            }
        }
    }
}
