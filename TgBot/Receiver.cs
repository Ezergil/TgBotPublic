using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TgBot.Base.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Args;
using TelegramBot.Infrastructure.Helpers;
using TelegramBot.Infrastructure.Interfaces;
using TelegramBot.Infrastructure.Services;

namespace TgBot
{
    public class Receiver : IHostedService
    {
        private static ITelegramBotClientAdapter _client;
        private readonly MessageHandlerCache _listenerCache;
        private readonly CachedRepository<Stats> _statsRepository;
        private readonly ILogger<Receiver> _logger;
        private readonly CommandProcessor _processor;

        public Receiver(ITelegramBotClientAdapter client, 
            CommandProcessor processor, 
            MessageHandlerCache listenerCache,
            CachedRepository<Stats> statsRepository,
            ILogger<Receiver> logger,
            IConfiguration configuration)
        {
            _client = client;
            _listenerCache = listenerCache;
            _statsRepository = statsRepository;
            _logger = logger;
            _processor = processor;
        }

        private async Task StartReceiving()
        {
            try
            {
                var me = await _client.GetMeAsync();
                Console.WriteLine($"Hello, World2! I am user {me.Id} and my name is {me.FirstName}.");
                _client.OnUpdate += Bot_OnMessage;
                _client.StartReceiving();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (_client.IsReceiving)
                    _client.StopReceiving();
                _logger.LogError(ex, "Smthn wrong, dude");
                throw;
            }
        }

        private async void Bot_OnMessage(object sender, UpdateEventArgs e)
        {
            try
            {
                await _listenerCache.Listen(e.Update);
                await ProcessCommand(e);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Smthn wrong, dude");
            }
        }

        private async Task ProcessCommand(UpdateEventArgs update)
        {
            var messageEventArgs = (MessageEventArgs)update;

            var message = messageEventArgs?.Message;
            if (message?.Text == null)
                return;
            string text = message.Text;
            var command = string.Empty;
            if (_processor.IsCommand(ref text, ref command))
            {
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("[ ]{2,}", options);
                text = regex.Replace(text, " ");
                var args = SplitCommandLine(text).ToList();
                args.Insert(0, command);
                Console.WriteLine($"Command received : {command} {text}");

                try
                {
                    await _processor.HandleCommand(command, message, args);
                }
                catch
                {
                    await _client.SendTextMessageAsync(
                         message.Chat.Id,
                         "Unknown error occurred");
                    throw;
                }
            }
        }

        public void Start()
        {
            Thread.Sleep(int.MaxValue);
        }

        #region Command args
        private static IEnumerable<string> SplitCommandLine(string commandLine)
        {
            bool inQuotes = false;

            return Split(commandLine, c =>
                {
                    if (c == '\"')
                        inQuotes = !inQuotes;

                    return !inQuotes && c == ' ';
                })
                .Select(arg => TrimMatchingQuotes(arg.Trim(), '\"'))
                .Where(arg => !string.IsNullOrEmpty(arg));
        }

        private static IEnumerable<string> Split(string str,
            Func<char, bool> controller)
        {
            int nextPiece = 0;

            for (int c = 0; c < str.Length; c++)
            {
                if (controller(str[c]))
                {
                    yield return str.Substring(nextPiece, c - nextPiece);
                    nextPiece = c + 1;
                }
            }

            yield return str.Substring(nextPiece);
        }

        private static string TrimMatchingQuotes(string input, char quote)
        {
            if ((input.Length >= 2) &&
                (input[0] == quote) && (input[input.Length - 1] == quote))
                return input.Substring(1, input.Length - 2);

            return input;
        }
        #endregion

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Application is starting..");
            await RetryPolicy.AsyncPolicy<Exception>().ExecuteAsync(async () => await StartReceiving());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _statsRepository.ProcessChangesFromCache();
            _logger.LogWarning("Application is stopping..");
            return Task.CompletedTask;
        }
    }
}
