using Telegram.Bot;
using Microsoft.Extensions.Configuration;


namespace TgBot
{
    public class TelegramBotClientContext
    {
        private readonly IConfiguration _configuration;

        public TelegramBotClientContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private ITelegramBotClient _client;
        public ITelegramBotClient Current
        {
            get
            {
                if (_client != null) return _client;
                    _client = new TelegramBotClient(_configuration["Token"]);
                return _client;
            }
        }
    }
}
