using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.DynamicProxy;
using Telegram.Bot.Types;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Helpers;
using TelegramBot.Infrastructure.Interfaces;

namespace TelegramBot.Infrastructure.Base
{
    [Intercept(typeof(CallLogger))]
    public abstract class CommandHandler
    {
        public abstract string[] PossibleCommands { get; }
        public abstract string Usage { get; }
        protected virtual bool Public => true;
        private IRepository<CommandPermission> _repository;


        protected readonly ITelegramBotClientAdapter Client;

        protected CommandHandler(ITelegramBotClientAdapter client)
        {
            Client = client;
        }

        public void SetPermissionRepository(IRepository<CommandPermission> repository)
        {
            _repository = repository;
        }

        protected async Task<bool> ValidatePermissions(TelegramMessage message, bool? publicOverride = null)
        {
            var hasAccess = _repository == null ? true : _repository.SingleOrDefault(p=> 
                p.UserId == message.From.Id && p.ChatId == message.Chat.Id &&
                GetType().Name.StartsWith(p.CommandHandlerName))?.HasAccess;
            if ((publicOverride ?? Public) && (!hasAccess.HasValue || hasAccess.Value))
                return true;
            var chatAdministratorIds = (await Client.GetChatAdministratorsAsync(message.Chat).ConfigureAwait(false)).
                Select(a => a.User.Id);
            return chatAdministratorIds.Contains(message.From.Id) && (!hasAccess.HasValue || hasAccess.Value)
                   || hasAccess.HasValue && hasAccess.Value;
        }

        private async Task PrintUsage(long chatId)
        {
            if (!string.IsNullOrEmpty(Usage))
            {
                await Client.SendTextMessageAsync(chatId, Usage).ConfigureAwait(false);
            }
        }

        public async Task Handle(Message message, List<string> args)
        {
            var msg = new TelegramMessage();
            msg.CopyPropertiesFrom(message);
            if (args.Count == 2 && args[1] == "help" || !ValidateArgs(msg, args))
            {
                await PrintUsage(message.Chat.Id).ConfigureAwait(false);
            }
            else
            {
                if (await ValidatePermissions(msg).ConfigureAwait(false))
                    await HandleCommand(msg, args).ConfigureAwait(false);
                else
                    await Client.SendTextMessageAsync(msg.Chat.Id, "You are not allowed to use this command")
                        .ConfigureAwait(false);
            }
        }

        protected abstract Task HandleCommand(TelegramMessage message, List<string> args);
        protected virtual bool ValidateArgs(TelegramMessage message, List<string> args) => true;
    }
}
