using System;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Infrastructure.Interfaces
{
    public interface ITelegramBotClientAdapter
    {
		event EventHandler<UpdateEventArgs> OnUpdate;
		bool IsReceiving { get; }

		Task SendTextMessageAsync(long chatId, string text, ParseMode parseMode = ParseMode.Default,
            int replyToMessageId = 0, IReplyMarkup replyMarkup = null);
        Task<Message> SendPollAsync(ChatId chat, string question, string[] options, bool isAnonymous = false, 
            bool allowsMultipleAnswers = false);
        Task<ChatMember[]> GetChatAdministratorsAsync(ChatId chat);
        Task DeleteMessageAsync(ChatId chat, int messageId);
        Task<Message> ForwardMessageAsync(long chatId, ChatId fromChatId, int messageId);
        Task PinChatMessageAsync(ChatId chatId, int rulesId, bool disableNotification);
        Task UnpinChatMessageAsync(ChatId chatId);
        Task StopPollAsync(ChatId chat, int messageId);
        Task LeaveChatAsync(ChatId chatId);
        Task<ChatMember> GetChatMemberAsync(ChatId chatId, int userId);
        Task<Chat> GetChatAsync(long chatId);
		Task<User> GetMeAsync();
		void StartReceiving();
		void StopReceiving();
		Task SendAnimationAsync(long chatId, InputOnlineFile animation);
    }
}