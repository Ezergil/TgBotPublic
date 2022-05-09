using System.Collections.Generic;
using Autofac;
using TgBot.Base.Entities;
using Moq;
using TelegramBot.Infrastructure.Database.Interfaces;

namespace TgBot.Tests
{
    public static class TestsIoC
    {
        public static void ConfigureTests(ContainerBuilder builder)
        {
            var statsRepositoryMock = new Mock<IRepository<Stats>>();
            statsRepositoryMock.Setup(r => r.GetAll()).Returns(new List<Stats>());
            builder.Register(c => statsRepositoryMock.Object).As<IRepository<Stats>>();
        }
    }
}