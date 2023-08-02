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
                var result=  await this.currencyService.CreateAsync(currencyDto, loggedUser);
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
                var result = await this.currencyService.UpdateAsync(id, currencyDto, loggedUser);
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

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var result = await this.currencyService.DeleteAsync(id, loggedUser);
                if (!result.IsSuccessful)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, result.Message);
                }

                return StatusCode(StatusCodes.Status200OK, result.Message);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
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