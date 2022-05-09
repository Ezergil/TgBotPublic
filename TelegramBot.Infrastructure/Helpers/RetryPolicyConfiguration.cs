namespace TelegramBot.Infrastructure.Helpers
{
	public class RetryPolicyConfiguration
    {
        public static int RetryCount {internal get; set;}
        public static int RetryDelay {internal get; set;}
        public static int AsyncRetryCount {internal get; set;}
        public static int AsyncRetryDelay {internal get; set;}
    }
}
