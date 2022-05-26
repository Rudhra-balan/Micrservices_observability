using System;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Repository.Interface
{
    public interface IRetryPolicy
    {
        void Execute(Action operation);

        TResult Execute<TResult>(Func<TResult> operation);

        Task ExecuteAsync(Func<Task> operation);

        Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation);
    }
}
