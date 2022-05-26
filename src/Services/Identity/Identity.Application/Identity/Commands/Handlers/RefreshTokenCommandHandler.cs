using BuildingBlocks;
using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.Logging;
using BuildingBlocks.Repository;
using BuildingBlocks.TokenHandler;
using Dapper;
using Identity.Application.Identity.EntityModel;
using Identity.Application.Identity.Exceptions;
using Identity.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.Application.Identity.Commands.Handlers;

public class RefreshTokenCommandHandler : SqlServerConnectionFactory, ICommandHandler<RefreshTokenCommand>
{
    private readonly ILogger<RefreshTokenCommandHandler> _logger;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(ILogger<RefreshTokenCommandHandler> logger, IOptions<DatabaseConnection> databaseOptions, ITokenService tokenService) : base(databaseOptions)
    {
        _logger = logger;
        _tokenService = tokenService;
    }
 
    public async Task<TResponse> HandleAsync<TResponse>(RefreshTokenCommand command, CancellationToken token)
    {
        try
        {
            var claimsPrincipal = _tokenService.GetPrincipalFromToken(command.AccessToken);
            var userCliam = claimsPrincipal?.Claims.First(c => c.Type == ClaimsIdentity.NameId);

            if (userCliam == null)
            {
                throw new RefreshTokenException(ResponseMessage.UnknownApiError);
            }

            var refreshToken = await Connection(async conn =>
            {
                //Authenticate & Get user information for valid user information
                var userEntity = await conn.QueryFirstOrDefaultAsync<UserModel>(SqlConstant.SqlConstant.GetUserInfoById, new
                {
                    iUserID = int.Parse(userCliam.Value)
                });

                if (userEntity == null)
                {
                    throw new RefreshTokenException("invalid token. Please obtain a new token.");
                }

                var token = _tokenService.GenerateAccessToken(userEntity);
                //Authenticate & Get user information for valid user information
                var tokenExpiryTime = await conn.QueryFirstOrDefaultAsync<int>(SqlConstant.SqlConstant.SQL_REFRESH_TOKEN_EXPIRY_TIME, new
                {
                    iRefreshToken = token.RefreshToken,
                    iUserId = userEntity.UserID
                });

                if (tokenExpiryTime == -1 || tokenExpiryTime > 180)
                {
                    throw new RefreshTokenException("Token Expired. Please obtain a new token.");
                }

                using var transaction = conn.BeginTransaction();

                var affectedRows = await conn.ExecuteAsync(SqlConstant.SqlConstant.SQL_UPDATE_AUTHENTICATION_TOKEN, new
                {
                    iUserId = userEntity.UserID,
                    iAccessToken = token.AccessToken,
                    iNewRefreshToken = token.RefreshToken,
                    iOldRefreshToken = command.RefreshToken,
                }, transaction);

                if (affectedRows == 0)
                {
                    transaction.Rollback();
                    throw new RefreshTokenException("Invalid or Expired Token. Please try again.");
                }

                transaction.Commit();

                return token;

            });

            return (TResponse)Convert.ChangeType(refreshToken, refreshToken.GetType());
        }
        catch (Exception ex)
        {
            _logger.LogFormatError(ex);
            throw;
        }
    }
}