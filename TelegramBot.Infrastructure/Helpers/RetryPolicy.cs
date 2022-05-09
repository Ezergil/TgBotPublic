using System;
using Polly;
using Polly.Retry;

namespace TelegramBot.Infrastructure.Helpers
{
	public class RetryPolicy
    {
        public static Policy Policy<TExeption>() where TExeption: Exception {
            return Polly.Policy.Handle<TExeption>().WaitAndRetry(
            RetryPolicyConfiguration.RetryCount, _ => TimeSpan.
            FromMilliseconds(RetryPolicyConfiguration.RetryDelay));
        }

        public static AsyncRetryPolicy AsyncPolicy<TExeption>() where TExeption: Exception 
        {
            return Polly.Policy.Handle<TExeption>().
            WaitAndRetryAsync(RetryPolicyConfiguration.AsyncRetryCount, _ =>
            TimeSpan.FromMilliseconds(RetryPolicyConfiguration.AsyncRetryDelay));
        }
    }
}
