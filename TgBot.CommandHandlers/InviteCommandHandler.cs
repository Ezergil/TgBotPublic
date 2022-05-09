using System.Collections.Generic;
using System.Threading.Tasks;
using TgBot.Base.Interfaces;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.DTO;
using TelegramBot.Infrastructure.Interfaces;

namespace TgBot.CommandHandlers
{
    public class InviteCommandHandler : CommandHandler
    {
        private readonly IPollService _service;

        public override string[] PossibleCommands => new[] { "/invite", "/invite@ppl_inviter_bot", "бот, голосование по" };
        public override string Usage => "Usage: \r\n/invite @<username> - create poll to vote for specific user.";
        protected override bool Public => false;

        public InviteCommandHandler(IPollService service,
            ITelegramBotClientAdapter client) : base(client)
        {
            _service = service;
        }
        
        protected override async Task HandleCommand(TelegramMessage message, List<string> args)
        {
            var nickName = args[1];
            await _service.CreatePoll(message.Chat.Id, $"#Голосование по кандидату в чат : {nickName}",
                new[]
                {
                    "Вето. Я против присоединения",
                    "Мне эта идея не очень. Выражаю сомнения. На решение админа",
                    "Выражаю симпатию, но лично приглашать не готов",
                    "Категорически за. Готов(а) лично пригласить в наш чат",
                    "Я против добавления новых участников в чат",
                    "Хочу посмотреть результат"
                }, true, message.From.Id);
        }

        protected override bool ValidateArgs(TelegramMessage message, List<string> args)
        {
            return args.Count >= 2 && args[1].StartsWith("@");
        }
    }
}
