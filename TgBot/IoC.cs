using Autofac;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TgBot.Base.Interfaces;
using TgBot.CommandHandlers;
using TgBot.Jobs;
using TgBot.MessageHandlers;
using TgBot.Services;
using Telegram.Bot;
using TelegramBot.Infrastructure.Database;
using TelegramBot.Infrastructure.Extensions.DependencyInjection;
using TelegramBot.Infrastructure.Helpers;
using TgBot.DB;

namespace TgBot
{
    public static class IoC
    {
        public static void Configure(ContainerBuilder builder)
        {
            builder.RegisterType<Receiver>().AsSelf().SingleInstance();
            
            builder.RegisterType<TelegramBotClientContext>().SingleInstance();
            builder.Register(c => c.Resolve<TelegramBotClientContext>().Current).As<ITelegramBotClient>()
                .SingleInstance();
            
            builder.RegisterType<DeleteMessageService>().AsSelf().SingleInstance();
            builder.RegisterType<DeletePolicyService>().AsSelf().SingleInstance();
            builder.RegisterType<DbContextOptionsBuilder>();
            builder.RegisterType<TgBotContext>().As<BotContext>();
            builder.RegisterType<PollService>().As<IPollService>().SingleInstance();
            builder.RegisterType<UserService>().As<IUserService>().SingleInstance();
            builder.RegisterType<MentionUsersService>().As<IMentionUsersService>().SingleInstance();


            builder.RegisterTelegramBot();
            builder.RegisterJobs(new[] { Assembly.GetAssembly(typeof(DeleteOldMessagesJob)) });
            builder.RegisterMessageHandlers(new[] { Assembly.GetAssembly(typeof(DeletePolicyMessageHandler)) });
            builder.RegisterCommandHandlers(new[] { Assembly.GetAssembly(typeof(DeletePolicyCommandHandler)) });

            RetryPolicyConfiguration.AsyncRetryCount = 5;
            RetryPolicyConfiguration.AsyncRetryDelay = 200;
        }
    }
}
