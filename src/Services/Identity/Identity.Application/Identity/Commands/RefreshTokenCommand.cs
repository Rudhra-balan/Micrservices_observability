using BuildingBlocks.CQRS.Commands;

namespace Identity.Application.Identity.Commands;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand;