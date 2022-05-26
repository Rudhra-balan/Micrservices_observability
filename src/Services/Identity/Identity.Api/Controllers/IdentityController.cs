
using BuildingBlocks.CQRS.Commands;
using Identity.Application.Identity.Commands;
using Identity.Application.Identity.Dtos;
using Microsoft.AspNetCore.Mvc;


namespace Identity.Api.Controllers
{
    [ApiController]
    public class IdentityController  : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ILogger<IdentityController> _logger;
       

        public IdentityController(ICommandDispatcher commandDispatcher, ILogger<IdentityController> logger)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
      
        }

        [HttpPost(nameof(RegisterNewUser))]
        public async Task<AccessTokenModel> RegisterNewUser(RegisterNewUserCommand command)
        {
            using (_logger.BeginScope("User Registration {@command}", command))
                return await _commandDispatcher.SendAsync<RegisterNewUserCommand, AccessTokenModel>(command);
            

        }

        [HttpPost(nameof(Authenticate))]
        public async Task<AccessTokenModel> Authenticate(AuthenticateCommand command)
        {
            using (_logger.BeginScope("User Authenticate {@command}", command))
                return await _commandDispatcher.SendAsync<AuthenticateCommand, AccessTokenModel>(command);
        
        }

        [HttpPost(nameof(RefreshToken))]
        public async Task<AccessTokenModel> RefreshToken(RefreshTokenCommand command)
        {
            using (_logger.BeginScope("Refresh Token {@command}", command))
                return    await _commandDispatcher.SendAsync<RefreshTokenCommand, AccessTokenModel>(command);
        }
    }
}
