
using Business.Exceptions;
using Business.DTOs.Requests;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using Business.DTOs.Responses;
using Business.DTOs;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/currencies")]
    public class CurrencyApiController : ControllerBase
    {
        private readonly ICurrencyService currencyService;
        private readonly IUserService userService;

        public CurrencyApiController(
            ICurrencyService currencyService,
            IUserService userService)
        {
            this.currencyService = currencyService;
            this.userService = userService;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCurrencyDto currencyDto)
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
                var result = this.currencyService.GetAll();
                
                return StatusCode(StatusCodes.Status200OK, result);
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
                var currencyDto = await this.currencyService.GetByIdAsync(id);
                
                return StatusCode(StatusCodes.Status200OK, currencyDto);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] CreateCurrencyDto currencyDto)
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
        private async Task<Response<GetUserDto>> FindLoggedUserAsync()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = await this.userService.GetByUsernameAsync(loggedUsersUsername);
            return loggedUser;
        }
    }
}