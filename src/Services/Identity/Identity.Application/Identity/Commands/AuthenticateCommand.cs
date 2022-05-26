using BuildingBlocks.CQRS.Commands;

namespace Identity.Application.Identity.Commands;

public record AuthenticateCommand(string Username,string Password) : ICommand;