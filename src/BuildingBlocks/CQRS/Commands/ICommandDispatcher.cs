using System.Threading.Tasks;

namespace BuildingBlocks.CQRS.Commands;

public interface ICommandDispatcher
{
   Task<Tout> SendAsync<T, Tout>(T command) where T : class,  ICommand ;
}