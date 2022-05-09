using System;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Invocable;
using TgBot.Base.Entities;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.Jobs
{
    public class DeleteOldMessagesJob : IInvocable
    {
        private readonly IRepository<MessageEntry> _repository;
        private readonly ITelegramBotClientAdapter _client;

        public DeleteOldMessagesJob(IRepository<MessageEntry> repository,
            ITelegramBotClientAdapter client)
        {
            _repository = repository;
            _client = client;
        }

        public async Task Invoke()
        {
            await DeleteMatchingMessages();
        }

        private async Task Delete(MessageEntry message)
        {
            try
            {
                _repository.Delete(message);
                await _client.ForwardMessageAsync(160997836, message.ChatId, message.MessageId);
                await _client.ForwardMessageAsync(153370029, message.ChatId, message.MessageId);
                await _client.DeleteMessageAsync(message.ChatId, message.MessageId);
            }
            catch
            {
                //Ok, it already was deleted
            }
        }

        private Task DeleteMatchingMessages()
        {
            var deleteTasks = _repository.Find(m => m.LiveUntilUtc < DateTime.UtcNow).
                Select(Delete);
            return Task.WhenAll(deleteTasks);
        }
    }
}