using AutoMapper;
using Business.DTOs;
using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/hisrories")]
    public class HistoryApiController : ControllerBase
    {
        private readonly IHistoryService historyService;
        private readonly IAuthManager authManager;
        private readonly IMapper mapper;

        public HistoryApiController(
            IHistoryService historyService,
            IAuthManager authManager,
            IMapper mapper)
        {
            this.historyService = historyService;
            this.authManager = authManager;
            this.mapper = mapper;
        }

        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> GetbyIdAsync(int id)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var history = await this.historyService.GetByIdAsync(id, loggedUser);
                
                return StatusCode(StatusCodes.Status200OK, history);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }

        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetHistoryAsync([FromQuery] HistoryQueryParameters historyQueryParameters) 
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var history = await this.historyService.FilterByAsync(historyQueryParameters, loggedUser);
                
                return StatusCode(StatusCodes.Status200OK, history);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }

        }

        private async Task<User> FindLoggedUserAsync()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = await authManager.TryGetUserByUsernameAsync(loggedUsersUsername);
            return loggedUser;
        }
    }
}
