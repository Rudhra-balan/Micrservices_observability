using BuildingBlocks;
using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.Logging;
using BuildingBlocks.Repository;
using Dapper;
using DotNetCore.CAP;
using Identity.Application.Identity.EntityModel;
using Identity.Application.Identity.Exceptions;
using Identity.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net;

namespace Identity.Application.Identity.Commands.Handlers;

public class AuthenticateCommandHandler : SqlServerConnectionFactory, ICommandHandler<AuthenticateCommand>
{
    private readonly ILogger<AuthenticateCommandHandler> _logger;
    private readonly ITokenService _tokenService;

    private readonly ICapPublisher _capPublisher;
    public AuthenticateCommandHandler(ILogger<AuthenticateCommandHandler> logger, 
                                      IOptions<DatabaseConnection> databaseOptions,
                                      ITokenService tokenService,
                                      ICapPublisher capPublisher) : base(databaseOptions)
    {
        _logger = logger;
        _tokenService = tokenService;
        _capPublisher = capPublisher;
        
    }

   
    public async Task<TResponse> HandleAsync<TResponse>(AuthenticateCommand command, CancellationToken token)
    {
        try
        {
             var tokenModel =  await Connection(async conn =>
                {
                    
                    //Authenticate & Get user information for valid user information
                    var userEntity = await conn.QueryFirstOrDefaultAsync<UserModel>(SqlConstant.SqlConstant.Authentication, new
                    {
                        iUsername = command.Username, iPassword = command.Password
                    });

                    if (userEntity == null)
                        throw new AuthentioncationException("The username or password you entered is incorrect.please try again.", HttpStatusCode.Unauthorized);

                    var tokenModel = _tokenService.GenerateAccessToken(userEntity);

                    using var transaction = conn.BeginTransaction();

                    var affectedRows = await conn.ExecuteAsync(SqlConstant.SqlConstant.InsertRefreshToken, new
                    {
                        iUserId = userEntity.UserID,
                        iAccessToken = tokenModel.AccessToken,
                        iRefreshToken = tokenModel.RefreshToken,

                    }, transaction);

                    if (affectedRows == 0)
                    {
                        transaction.Rollback();
                        throw new RefreshTokenException("An error occured while generating access token. Please try again.");
                    }

                    transaction.Commit();

                    return tokenModel;

                    
                });

           return (TResponse)Convert.ChangeType(tokenModel, tokenModel.GetType());
        }
        catch (Exception ex)
        {
            _logger.LogFormatError(ex);
            throw;
        }
    }
}