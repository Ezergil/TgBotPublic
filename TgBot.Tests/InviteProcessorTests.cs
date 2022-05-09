using Autofac;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TelegramBot.Infrastructure.Services;

namespace TgBot.Tests
{
    class InviteProcessorTests
    {
        private IContainer _container;

        [SetUp]
        public void Initialize()
        {
            var builder = new ContainerBuilder();
            IoC.Configure(builder);
            _container = builder.Build();
        }

        [Test]
        public async Task AssertCorrectBehaviorForZeroArgumentsProvided()
        {
            var cache = _container.Resolve<CommandHandlerCache>();
            var processor = cache.GetCommandHandler("/invite");
            await processor.Handle(new Telegram.Bot.Types.Message(), new List<string> { "/invite" });
            //_clientMock.Verify(x => x.SendTextMessageAsync(new Telegram.TgBot.Types.ChatId(1),
            //    "Provide nickname, e.g. /invite @test", It.IsAny<ParseMode>(), It.IsAny<bool>(), It.IsAny<bool>(),
            //    It.IsAny<int>(), It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
