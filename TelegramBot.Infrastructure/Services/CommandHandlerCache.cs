using System;
using System.Collections.Generic;
using System.Linq;
using TelegramBot.Infrastructure.Base;

namespace TelegramBot.Infrastructure.Services
{
    public sealed class CommandHandlerCache
    {
        private readonly Func<IEnumerable<CommandHandler>> _factory;
        private IEnumerable<CommandHandler> _commandHandlers;

        public CommandHandlerCache(Func<IEnumerable<CommandHandler>> factory)
        {
            _factory = factory;
        }

        public CommandHandler GetCommandHandler(string actionName)
        {
            return GetAll().SingleOrDefault(h => h.PossibleCommands.Contains(actionName));
        }

        public IEnumerable<CommandHandler> GetAll()
        {
            if (_commandHandlers == null)
                _commandHandlers = _factory();
            return _commandHandlers.ToList();
        }
    }
}
