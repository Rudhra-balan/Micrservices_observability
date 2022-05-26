using BuildingBlocks.CQRS.Commands;

namespace Transaction.Application.Identity.Commands;

public record DepositCommand(decimal Amount, string Currency) : ICommand;