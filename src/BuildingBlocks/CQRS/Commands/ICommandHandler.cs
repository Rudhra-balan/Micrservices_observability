using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.CQRS.Commands;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
   
    Task<TResponse> HandleAsync<TResponse>(TCommand command, CancellationToken token);
}