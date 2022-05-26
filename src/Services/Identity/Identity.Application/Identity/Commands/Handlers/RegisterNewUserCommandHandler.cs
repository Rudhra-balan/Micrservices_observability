using BuildingBlocks;
using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.Logging;
using BuildingBlocks.Repository;
using Dapper;
using Identity.Application.Identity.EntityModel;
using Identity.Application.Identity.Exceptions;
using Identity.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.Application.Identity.Commands.Handlers;

public class RegisterNewUserCommandHandler : SqlServerConnectionFactory, ICommandHandler<RegisterNewUserCommand>
{
    private readonly ILogger<RegisterNewUserCommandHandler> _logger;
    private readonly ITokenService _tokenService;

    public RegisterNewUserCommandHandler(ILogger<RegisterNewUserCommandHandler> logger, IOptions<DatabaseConnection> databaseOptions, ITokenService tokenService) : base(databaseOptions)
    {
        _logger = logger;
        _tokenService = tokenService;
    }

    public async Task<TResponse> HandleAsync<TResponse>(RegisterNewUserCommand command, CancellationToken token)
    {
        try
        {


            var userInfo = await Connection(async conn =>
            {

                using var transaction = conn.BeginTransaction();

                var affectedRows = await conn.ExecuteAsync(SqlConstant.SqlConstant.RegisterUser, new
                {
                    @iUserName = command.Username,
                    @iFirstName = command.FirstName,
                    @iLastName = command.LastName,
                    @iUserEmail = command.Email,
                    @iPassword = command.Password

                }, transaction);

                if (affectedRows == 0)
                {
                    transaction.Rollback();
                    throw new RegisterIdentityUserException("An error occured while registering user. Please try again.");
                }

                transaction.Commit();

            //Authenticate & Get user information for valid user information
            var userEntity = await conn.QueryFirstOrDefaultAsync<UserModel>(SqlConstant.SqlConstant.Authentication, new
                {
                    iUsername = command.Username,
                    iPassword = command.Password
                });

                return userEntity;

            });

            var tokenModel = _tokenService.GenerateAccessToken(userInfo);

            return (TResponse)Convert.ChangeType(tokenModel, tokenModel.GetType());
        }
        catch (Exception ex)
        {
            _logger.LogFormatError(ex);
            throw;
        }
    }
}