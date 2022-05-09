using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Infrastructure.Interfaces;

namespace TelegramBot.Infrastructure.Helpers
{
    public class TelegramBotClientAdapter : ITelegramBotClientAdapter
    {
        private readonly ITelegramBotClient _client;
        private static volatile object _objectLock = new object();
		public event EventHandler<UpdateEventArgs> OnUpdate 
        {
            add
            {
                lock (_objectLock)
                {
                    _client.OnUpdate += value;
                }
            }
            remove
            {
                lock (_objectLock)
                {
                    _client.OnUpdate -= value;
                }
            }
        }

		public bool IsReceiving => _client.IsReceiving;

		public TelegramBotClientAdapter(ITelegramBotClient client)
        {
            _client = client;
        }

        public async Task SendTextMessageAsync(long chatId, string text,
            ParseMode parseMode = ParseMode.Default, int replyToMessageId = default, IReplyMarkup replyMarkup = default)
            => await RetryPolicy.AsyncPolicy<Exception>().ExecuteAsync(async () =>
                await _client.SendTextMessageAsync(chatId, text, parseMode, replyToMessageId: replyToMessageId,
                    replyMarkup: replyMarkup).ConfigureAwait(false)).ConfigureAwait(false);

        public async Task SendAnimationAsync(long chatId, InputOnlineFile animation) => await RetryPolicy.AsyncPolicy<Exception>()
            .ExecuteAsync(async () =>
                await _client.SendAnimationAsync(chatId, animation));
        
        public async Task<Message> SendPollAsync(ChatId chat, string question, 
            string[] options, bool isAnonymous = false, bool allowsMultipleAnswers = false)
            => await RetryPolicy.AsyncPolicy<Exception>().ExecuteAsync(async () =>
                await _client.SendPollAsync(chat, question, options, isAnonymous: isAnonymous, 
                    allowsMultipleAnswers: allowsMultipleAnswers));

        public async Task StopPollAsync(ChatId chat, int messageId)
            => await RetryPolicy.AsyncPolicy<Exception>().ExecuteAsync(async () =>
                await _client.StopPollAsync(chat, messageId).ConfigureAwait(false)).ConfigureAwait(false);

        public async Task<ChatMember[]> GetChatAdministratorsAsync(ChatId chat)
            => await RetryPolicy.AsyncPolicy<Exception>().ExecuteAsync(async () =>
                await _client.GetChatAdministratorsAsync(chat).ConfigureAwait(false)).ConfigureAwait(false);

        public async Task DeleteMessageAsync(ChatId chat, int messageId)
            => await RetryPolicy.AsyncPolicy<Exception>().ExecuteAsync(async () =>
                await _client.DeleteMessageAsync(chat, messageId).ConfigureAwait(false)).ConfigureAwait(false);

        public async Task<Message> ForwardMessageAsync(long chatId, ChatId fromChatId, int messageId)
            => await RetryPolicy.AsyncPolicy<Exception>().ExecuteAsync(async () =>
                    await _client.ForwardMessageAsync(chatId, fromChatId, messageId).ConfigureAwait(false))
                .ConfigureAwait(false);

        public async Task PinChatMessageAsync(ChatId chatId, int rulesId, bool disableNotification)
            => await RetryPolicy.AsyncPolicy<Exception>().ExecuteAsync(async () =>
                    await _client.PinChatMessageAsync(chatId, rulesId, disableNotification).ConfigureAwait(false))
                .ConfigureAwait(false);

        public async Task UnpinChatMessageAsync(ChatId chatId)
            => await RetryPolicy.AsyncPolicy<Exception>().ExecuteAsync(async () =>
                await _client.UnpinChatMessageAsync(chatId).ConfigureAwait(false)).ConfigureAwait(false);

        public async Task LeaveChatAsync(ChatId chatId)
            => await RetryPolicy.AsyncPolicy<Exception>().ExecuteAsync(async () =>
                await _client.LeaveChatAsync(chatId).ConfigureAwait(false)).ConfigureAwait(false);
        
        public async Task<ChatMember> GetChatMemberAsync(ChatId chatId, int userId)
            => await RetryPolicy.AsyncPolicy<Exception>().ExecuteAsync(async () =>
                await _client.GetChatMemberAsync(chatId, userId).ConfigureAwait(false)).ConfigureAwait(false);

        public async Task<Chat> GetChatAsync(long chatId)
            => await RetryPolicy.AsyncPolicy<Exception>().ExecuteAsync(async () =>
                await _client.GetChatAsync(chatId).ConfigureAwait(false)).ConfigureAwait(false);

		public async Task<User> GetMeAsync() 
            => await RetryPolicy.AsyncPolicy<Exception>().ExecuteAsync(async () =>
                await _client.GetMeAsync().ConfigureAwait(false)).ConfigureAwait(false);

		public void StartReceiving() => _client.StartReceiving();

		public void StopReceiving() => _client.StopReceiving();

	}
}