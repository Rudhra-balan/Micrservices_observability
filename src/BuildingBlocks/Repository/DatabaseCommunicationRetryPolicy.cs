using BuildingBlocks.Repository.Interface;
using Microsoft.Data.Sqlite;
using Polly;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BuildingBlocks.Repository
{
    public class DatabaseCommunicationRetryPolicy : IRetryPolicy
    {
        private const int RetryCount = 3;
        private const int WaitBetweenRetriesInMilliseconds = 1000;
        private readonly AsyncPolicy _retryPolicyAsync;
        private readonly Policy _retryPolicy;

        public DatabaseCommunicationRetryPolicy()
        {
            _retryPolicyAsync = Policy
                .Handle<SqliteException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(
                    retryCount: RetryCount,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(WaitBetweenRetriesInMilliseconds),
                    onRetry: LogRetryAction

                );

            _retryPolicy = Policy
                .Handle<SqliteException>()
                .Or<TimeoutException>()
                .WaitAndRetry(
                    retryCount: RetryCount,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(WaitBetweenRetriesInMilliseconds),
                    onRetry: LogRetryAction
                );
        }

        private void LogRetryAction(System.Exception exception, TimeSpan sleepTime, int reattemptCount, Context context) =>
       Log.Warning(
           exception,
           $"Transient DB Failure while executing query, error number: {((SqliteException)exception).SqliteErrorCode}; reattempt number: {reattemptCount}");


        public void Execute(Action operation)
        {
            _retryPolicy.Execute(operation.Invoke);
        }

        public TResult Execute<TResult>(Func<TResult> operation)
        {
            return _retryPolicy.Execute(() => operation.Invoke());
        }

        public async Task ExecuteAsync(Func<Task> operation)
        {
            await _retryPolicyAsync.ExecuteAsync(operation.Invoke);
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation)
        {
            return await _retryPolicyAsync.ExecuteAsync(operation.Invoke);
        }
    }
}
