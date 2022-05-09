using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class PinCommandHandler : CommandHandler
    {
        public override string[] PossibleCommands => new[] { "/pin", "бот, закрепи", "бот закрепи", "/unpin", "бот, открепи", "бот открепи" };
        public override string Usage =>
            "Usage: \r\nCommand /pin is used in reply to message you want to pin";

        protected override bool Public => false;
        public PinCommandHandler(ITelegramBotClientAdapter client) : base(client)
        {
        }
        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            try
            {
                if (new []{"/pin","бот, закрепи","бот закрепи"}.Contains(args[0]))
                {
                    await Client.PinChatMessageAsync(message.Chat.Id, message.ReplyToMessage.MessageId, false);
                }
                else
                {
                    await Client.UnpinChatMessageAsync(message.Chat.Id);
                }
            }
            catch 
            {
                //
            }
        }

        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            return message.ReplyToMessage != null;
        }
    }
}
