using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/histories")]
    public class HistoryApiController : ControllerBase
    {
        private readonly IHistoryService historyService;
        private readonly IUserService userService;
        private readonly IUserRepository userRepository;

        public HistoryApiController(
            IHistoryService historyService,
            IUserService userService,
            IUserRepository userRepository)
        {
            this.historyService = historyService;
            this.userService = userService;
            this.userRepository = userRepository;
        }

        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> GetbyIdAsync(int id)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var result = await this.historyService.GetByIdAsync(id, loggedUser);
                if (!result.IsSuccessful)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, result.Message);
                }

                return StatusCode(StatusCodes.Status200OK, result.Data);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetHistoryAsync([FromQuery] HistoryQueryParameters historyQueryParameters) 
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var result = await this.historyService.FilterByAsync(historyQueryParameters, loggedUser);

                if (!result.IsSuccessful)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, result.Message);
                }

                return StatusCode(StatusCodes.Status200OK, result.Data);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        private async Task<User> FindLoggedUserAsync()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = await this.userRepository.GetByUsernameAsync(loggedUsersUsername);
            return loggedUser;
        }
    }
}
