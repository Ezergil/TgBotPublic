using System.Threading.Tasks;
using TgBot.Base.Entities;
using TelegramBot.Infrastructure.Database.Interfaces;

namespace TgBot.Services
{
    public class DeleteMessageService
    {
        private readonly IRepository<MessageEntry> _repository;

        public DeleteMessageService(IRepository<MessageEntry> repository)
        {
            _repository = repository;
        }

        public async Task AddMessageAsync(MessageEntry message)
        {
            await _repository.AddOrUpdateAsync(message);
        }

        public void RemoveMessage(long chatId, int messageId)
        {
            var message = _repository.SingleOrDefault(m => m.ChatId == chatId
                && m.MessageId == messageId);
            if (message != null)
            {
                _repository.Delete(message);
            }
        }
    }
}