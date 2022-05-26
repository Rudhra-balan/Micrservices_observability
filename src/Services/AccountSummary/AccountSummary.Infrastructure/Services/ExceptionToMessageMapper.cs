
using BuildingBlocks.CQRS.Events;
using BuildingBlocks.Exception;
using AccountSummary.Core.Events;

namespace AccountSummary.Infrastructure.Services;

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