using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.Infrastructure.Interfaces;

namespace TelegramBot.Infrastructure.Services
{
    public class CommandProcessor
    {
        public CommandProcessor(CommandHandlerCache cache, 
            ITelegramBotClientAdapter client)
        {
            _cache = cache;
            _client = client;
        }

        private readonly CommandHandlerCache _cache;
        private readonly ITelegramBotClientAdapter _client;

        public bool IsCommand(ref string str, ref string cmd)
        {
            foreach (var command in _cache.GetAll().SelectMany(handler => handler.PossibleCommands))
            {
                if (!str.StartsWith($"{command} ", StringComparison.InvariantCultureIgnoreCase) 
                    && !str.Equals(command, StringComparison.InvariantCultureIgnoreCase)) 
                    continue;
                str = Replace(str, command, string.Empty, StringComparison.InvariantCultureIgnoreCase);
                cmd = command;
                return true;
            }

            return false;
        }

        public async Task HandleCommand(string commandName, Message message, List<string> args)
        {
            var commandHandler = _cache.GetCommandHandler(commandName);

            if (commandHandler == null)
            {
                await _client.SendTextMessageAsync(message.Chat.Id,
                    $"Command {commandName} does not exist").ConfigureAwait(false);
                return;
            }
            await commandHandler.Handle(message, args).ConfigureAwait(false);
        }

        private string Replace(string original, string pattern, string replacement, StringComparison comparisonType, int stringBuilderInitialSize = -1)
        {
            if (original == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(pattern))
            {
                return original;
            }


            int posCurrent = 0;
            int lenPattern = pattern.Length;
            int idxNext = original.IndexOf(pattern, comparisonType);
            StringBuilder result = new StringBuilder(stringBuilderInitialSize < 0 ? Math.Min(4096, original.Length) : stringBuilderInitialSize);

            while (idxNext >= 0)
            {
                result.Append(original, posCurrent, idxNext - posCurrent);
                result.Append(replacement);

                posCurrent = idxNext + lenPattern;

                idxNext = original.IndexOf(pattern, posCurrent, comparisonType);
            }

            result.Append(original, posCurrent, original.Length - posCurrent);

            return result.ToString();
        }
    }
}

