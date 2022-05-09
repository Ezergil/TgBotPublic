using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class RulesCommandHandler : CommandHandler
    {
        public override string[] PossibleCommands => new[] { "/rules", "/rules@ppl_inviter_bot" };

        public override string Usage => "Usage: \r\n/rules - print rules. /rules set should be used first" +
            "\r\n/rules pin - print rules and pin 'em \r\n/rules set - usable by admins only. Set chat rules";

        private readonly IRepository<RulesMessage> _repository;

        public RulesCommandHandler(IRepository<RulesMessage> repository,
            ITelegramBotClientAdapter client) : base(client)
        {
            _repository = repository;
        }


        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            if (args.Count > 1 && args[1] == "set" && !await ValidatePermissions(message, false))
            {
                return;
            }
            var rules = new RulesMessage
            {
                ChatId = message.Chat.Id,
                Text = message.ReplyToMessage?.Text,
                Date = message.ReplyToMessage?.Date ?? new DateTime(),
                Id = message.ReplyToMessage?.MessageId ?? 0
            };
            if (args.Count == 2 && args[1] == "set" && message.ReplyToMessage != null)
            {
                await _repository.AddOrUpdateAsync(rules, r => r.ChatId == message.Chat.Id);
                await Client.SendTextMessageAsync(message.Chat.Id, "Success");
            }
            else
            {
                rules = _repository.SingleOrDefault(r => r.ChatId == message.Chat.Id);
                if (rules != null)
                {
                    if (args.Count == 2 && args[1] == "pin")
                    {
                        try
                        {
                            await Client.
                                PinChatMessageAsync(message.Chat, rules.Id, disableNotification: true);
                        }
                        catch
                        {
                            await Client.UnpinChatMessageAsync(message.Chat);
                            await Client.
                                PinChatMessageAsync(message.Chat, rules.Id, disableNotification: true);
                        }
                    }
                    await Client.SendTextMessageAsync(message.Chat.Id, rules.Text);
                }
            }
        }
    }
}
