using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using TgBot.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TelegramBot.Infrastructure.Database;
using TgBot.DB;

namespace TgBot
{
    public class Program
    {
        static void Main()
        {
            var hostBuilder = CreateHostBuilder();
            var host = hostBuilder.Build();
            var topLevelLogger = host.Services.GetService<ILogger<Program>>();

            var dbContext = host.Services.GetService<BotContext>();
            dbContext.Database.Migrate();
            host.Services.UseScheduler(scheduler =>
            {
                scheduler.
                    Schedule<ReminderJob>().
                    EveryMinute();
                scheduler.
                    Schedule<PollNotifierJob>().
                    Hourly();
                scheduler.
                    Schedule<StatsLeadersJob>().
                    Daily();
                scheduler.
                    Schedule<DeleteOldMessagesJob>().
                    EveryFiveSeconds().
                    PreventOverlapping(nameof(DeleteOldMessagesJob));
                scheduler.
                    Schedule<RemoveChatUserJob>().
                    Hourly();
            }).LogScheduledTaskProgress(host.Services.GetService<ILogger<IScheduler>>());
            try
            {
                host.Run();
            }
            catch (Exception ex)
            {
                topLevelLogger.LogError(ex, "Smthn wrong, dude");
            }
        }

        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder().
                ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile(@"Properties\launchsettings.json", optional: true);
                    configHost.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                }).ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile("appSettings.json", optional: false);
                    configApp.AddJsonFile(
                        $"appSettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        optional: false);
                    configApp.AddEnvironmentVariables();
                }).
                ConfigureLogging(c =>
                {
                    c.AddConsole();
                    c.SetMinimumLevel(LogLevel.Warning);
                }).
                UseServiceProviderFactory(new AutofacServiceProviderFactory()).
                ConfigureContainer<ContainerBuilder>(IoC.Configure).
                ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Receiver>();
                    services.AddScheduler();
                    services.AddDbContext<TgBotContext>(builder =>
                        builder.UseNpgsql(hostContext.Configuration.GetConnectionString("PostgreConnectionString")));
                }).
                UseConsoleLifetime();
    }
}
