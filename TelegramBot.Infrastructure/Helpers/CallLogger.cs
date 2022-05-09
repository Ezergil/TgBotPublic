using System;
using System.Linq;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TelegramBot.Infrastructure.Helpers
{
    public class CallLogger : IInterceptor
    {
        private readonly ILogger _logger;

        public CallLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            if (!invocation.Method.Name.StartsWith("get_") && !invocation.Method.Name.StartsWith("set_"))
            {
                _logger.LogInformation("Calling method {0} with parameters {1}... ",
                    invocation.Method.Name,
                    string.Join(", ", JsonConvert.SerializeObject(invocation.Arguments.Select(a => (a ?? "")).ToArray())));
            }
            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");
                throw;
            }
            if (!invocation.Method.Name.StartsWith("get_") && !invocation.Method.Name.StartsWith("set_"))
            {
                _logger.LogInformation("Done: result was {0}.");
            }
        }
    }
}