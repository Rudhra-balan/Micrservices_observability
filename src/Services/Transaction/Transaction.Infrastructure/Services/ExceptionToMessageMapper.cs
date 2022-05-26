
using BuildingBlocks.CQRS.Events;
using BuildingBlocks.Exception;


namespace Transaction.Infrastructure.Services;

public class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public IRejectedEvent Map(Exception exception, object message)
    {
        return exception switch
        {
            _ => null
        };
    }
}