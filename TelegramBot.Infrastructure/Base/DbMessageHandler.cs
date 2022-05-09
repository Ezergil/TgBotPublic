using Telegram.Bot.Types;
using TelegramBot.Infrastructure.Database.Entities;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TelegramBot.Infrastructure.Base
{
    public abstract class DbMessageHandler : MessageHandler<TelegramMessage>
    {

        private long _chatId;
        private readonly IRepository<ListenerState> _repository;
        private readonly string _className;

        protected DbMessageHandler(ITelegramBotClientAdapter client,
            IRepository<ListenerState> repository) : base(client)
        {
            _repository = repository;
            _className = GetType().Name;
        }

        public override bool Enabled { get => GetDbState(); set => SetDbState(value); }

        private void SetChatId(long chatId)
        {
            _chatId = chatId;
        }

        private bool GetDbState()
        {
            return _repository.SingleOrDefault(c => c.ListenerType == _className &&
                    c.ChatId == _chatId)?.State ?? false;
        }

        private void SetDbState(bool value)
        {
            var state = new ListenerState
            {
                State = value,
                ChatId = _chatId,
                ListenerType = _className
            };
            _repository.AddOrUpdateAsync(state, c => c.ListenerType == _className &&
                c.ChatId == _chatId).GetAwaiter().GetResult();
        }

        protected override void Setup(Update update)
        {
            if (update.Message == null)
                return;
            SetChatId(update.Message.Chat.Id);
        }
    }
}
