using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using Newtonsoft.Json;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;
using File = System.IO.File;

namespace TgBot.CommandHandlers
{
    public class WarnCommandHandler : CommandHandler
    {
        private static readonly string _warningsFile = "warnings_count.json";
        public override string[] PossibleCommands => new[] { "/warn@ppl_inviter_bot", "/warn" };
        public override string Usage => string.Empty;
        protected override bool Public => false;
        public WarnCommandHandler(ITelegramBotClientAdapter client) : base(client)
        {
        }
        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            var nickName = args[1];
            if (!File.Exists(_warningsFile))
            {
                var file = File.Create(_warningsFile);
                file.Close();
            }
            var warnings = JsonConvert.DeserializeObject<List<UserWarnings>>(File.ReadAllText(_warningsFile)) 
                ?? new List<UserWarnings>();
            var user = warnings.FirstOrDefault(u => u.UserName == nickName);
            if (user == null)
            {
                user = new UserWarnings { UserName = nickName, Count = 0 };
                warnings.Add(user);
            }
            user.Count++;
            File.WriteAllText(_warningsFile, JsonConvert.SerializeObject(warnings));
            if (user.Count < 3)
                await Client.SendTextMessageAsync(
                    message.Chat.Id,
                    $"User {nickName} receive warning. {3 - user.Count} till you'll be banned. Good luck avoiding it.");
            else
            {
                await Client.SendTextMessageAsync(
                    message.Chat.Id,
                    $"User {nickName} got banned. Sh$t happens.");
                //await _client.KickChatMemberAsync(message.Chat, )
            }
        }
        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            return args.Count >= 2 && args[1].StartsWith("@");
        }
    }
}
