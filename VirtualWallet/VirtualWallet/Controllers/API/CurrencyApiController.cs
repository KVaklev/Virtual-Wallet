using AutoMapper;
using Business.Dto;
using Business.Exceptions;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/currencies")]
    public class CurrencyApiController : ControllerBase
    {
        private readonly ICurrencyService currencyService;
        private readonly IUserService userService;
        private readonly IAuthManager authManager;

        public CurrencyApiController(
            ICurrencyService currencyService,
            IUserService userService,
            IAuthManager authManager)
        {
            this.currencyService = currencyService;
            this.userService = userService;
            this.authManager = authManager;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreateAsync([FromBody] CurrencyDto currencyDto)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                await this.currencyService.CreateAsync(currencyDto, loggedUser);

                return StatusCode(StatusCodes.Status200OK, currencyDto);
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

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var currencies = this.currencyService.GetAll();
                return StatusCode(StatusCodes.Status200OK, currencies);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            try
            {
                var currency = await this.currencyService.GetByIdAsync(id);
                
                return StatusCode(StatusCodes.Status200OK, currency);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] CurrencyDto currencyDto)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var updateCurencyDto = await this.currencyService.UpdateAsync(id, currencyDto, loggedUser);

                return StatusCode(StatusCodes.Status200OK, updateCurencyDto);
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

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var isDeleted = await this.currencyService.DeleteAsync(id, loggedUser);

                return StatusCode(StatusCodes.Status200OK, isDeleted);
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