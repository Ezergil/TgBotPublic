using Autofac;
using NUnit.Framework;
using System;
using TgBot.CommandHandlers;
using TelegramBot.Infrastructure.Services;

namespace TgBot.Tests
{
    public class ProcessorCacheTests
    {
        private IContainer _container;

        [SetUp]
        public void Initialize()
        {
            var builder = new ContainerBuilder();
            IoC.Configure(builder);
            TestsIoC.ConfigureTests(builder);
            _container = builder.Build();
        }

        [TestCase("/warn", typeof(WarnCommandHandler))]
        [TestCase("/invite", typeof(InviteCommandHandler))]
        [TestCase("/artem", typeof(ArtemCommandHandler))]
        [TestCase("/deletepolicy", typeof(DeletePolicyCommandHandler))]
        public void AssertProcessorFactoryReturnsCorrectProcessor(string command, Type processorType)
        {
            var cache = _container.Resolve<CommandHandlerCache>();
            Assert.True(cache.GetCommandHandler(command).GetType() == processorType);
        }

        [Test]
        public void MigrateData()
        {
            //var builder = new ContainerBuilder();
            //IoC.Configure(builder);
            //_container = builder.Build();
            //var sqliteRepository = _container.Resolve<IRepository<Bite>>();
            //var records = sqliteRepository.GetAll();
            //foreach (var r in records)
            //    r.Id = 0;
            //builder = new ContainerBuilder();
            //IoC.Configure(builder);
            //builder.RegisterType<PostgreBotContext>().As<BotContext>();
            //_container = builder.Build();
            //var postgreRepository = _container.Resolve<IRepository<Bite>>();
            //postgreRepository.AddRange(records);
        }
    }
}