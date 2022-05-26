
using BuildingBlocks.CQRS.Commands;
using Webhook.DomainCore.Model;

namespace Webhook.Application.Commands
{
    public record GrafanaRequestCommand(string? title, long? ruleId, string? ruleName, string? state, List<EvalMatch>? evalMatches, long? orgId,
        int? dashboardId, long? panelId, string? ruleUrl, string? message, object? tags = default) : ICommand;

 

}
