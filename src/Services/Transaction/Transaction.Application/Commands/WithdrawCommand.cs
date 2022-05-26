using BuildingBlocks.CQRS.Commands;

namespace Transaction.Application.Identity.Commands;

public record WithdrawCommand(decimal Amount, string Currency) : ICommand;