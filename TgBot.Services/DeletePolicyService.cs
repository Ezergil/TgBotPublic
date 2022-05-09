using System.Collections.Generic;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using TgBot.Base.Enums;
using TelegramBot.Infrastructure.Database.Interfaces;

namespace TgBot.Services
{
    public class DeletePolicyService
    {
        private readonly IRepository<PolicySettings> _repository;
        public DeletePolicyService(IRepository<PolicySettings> repository)
        {
            _repository = repository;
        }

        public PolicySettings GetPolicySettings(long chatId, long userId, MessageType messageType)
        {
            var existingConfiguration = _repository.
                SingleOrDefault(c => c.ChatId == chatId && c.UserId == userId &&
                c.MessageType == messageType);
            return existingConfiguration;
        }

        public PolicySettings GetGlobalPolicySettings(long chatId, MessageType messageType)
        {
            return _repository.SingleOrDefault(c => c.ChatId == chatId && c.MessageType == messageType && c.UserId == 777) ?? new PolicySettings() { Period = PeriodType.Never, ChatId = chatId };
        }

        public IEnumerable<PolicySettings> GetForChat(long chatId)
        {
            return _repository.Find(p => p.ChatId == chatId);
        }

        public async Task AddOrUpdatePolicySettingsAsync(PolicySettings settings)
        {
            await _repository.AddOrUpdateAsync(settings, c => c.ChatId == settings.ChatId &&
                c.UserId == settings.UserId && c.MessageType == settings.MessageType);
        }
    }
}
