using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Microsoft.Extensions.Logging;
using Coravel.Invocable;
using Telegram.Bot;
using TelegramBot.Infrastructure.Base;
using TelegramBot.Infrastructure.Database;
using TelegramBot.Infrastructure.Database.Interfaces;
using TelegramBot.Infrastructure.Helpers;
using TelegramBot.Infrastructure.Interfaces;
using TelegramBot.Infrastructure.Services;

namespace TelegramBot.Infrastructure.Extensions.DependencyInjection
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterTelegramBot(this ContainerBuilder builder)
        {
            builder.Register(c => new CallLogger(c.Resolve<ILogger<CallLogger>>()));
            builder.RegisterType<CommandProcessor>().AsSelf().SingleInstance();
            builder.RegisterGeneric(typeof(CachedRepository<>)).AsSelf().SingleInstance();
            builder.RegisterGeneric(typeof(Cache<>)).AsSelf().SingleInstance();
            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IRepository<>));
            builder.Register((c) => new TelegramBotClientAdapter(c.Resolve<ITelegramBotClient>())).
                As<ITelegramBotClientAdapter>();
            builder.RegisterType<CommandHandlerCache>().AsSelf().SingleInstance();
            builder.RegisterType<MessageHandlerCache>().AsSelf().SingleInstance();
        }

        public static void RegisterCommandHandlers(this ContainerBuilder builder, Assembly[] assemblies)
        {
            builder.RegisterTypes(assemblies.Append(Assembly.GetExecutingAssembly()).SelectMany(a => a.GetTypes())
                    .Where(t => t.IsSubclassOf(typeof(CommandHandler))).ToArray()).AsSelf().As<CommandHandler>().
                OnActivating(e => (e.Instance as CommandHandler).SetPermissionRepository(e.Context.Resolve<IRepository<CommandPermission>>())).
                EnableClassInterceptors().SingleInstance();
        }

        public static void RegisterMessageHandlers(this ContainerBuilder builder, Assembly[] assemblies)
        {
            builder.RegisterTypes(assemblies.SelectMany(a => a.GetTypes()).Where(t =>
                    t.GetInterfaces().Any(i => i.IsAssignableFrom(typeof(IMessageHandler))) && !t.IsAbstract).ToArray())
                .AsSelf().As<IMessageHandler>().SingleInstance();
        }

        public static void RegisterJobs(this ContainerBuilder builder, Assembly[] assemblies)
        {
            builder.RegisterTypes(assemblies.SelectMany(a => a.GetTypes()).Where(t =>
                    t.GetInterfaces().Any(i => i.IsAssignableFrom(typeof(IInvocable))) && !t.IsAbstract).ToArray())
                .AsSelf().As<IInvocable>().SingleInstance();
        }
    }
}