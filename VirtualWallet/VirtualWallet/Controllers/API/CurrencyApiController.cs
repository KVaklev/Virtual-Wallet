using Business.DTOs.Requests;
using Business.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Business.Services.Helpers;
using System.Security.Claims;

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
            var loggedUser = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);

            if (!loggedUser.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, loggedUser.Message);
            }
            var result = await this.currencyService.CreateAsync(currencyDto, loggedUser.Data);
            if (!result.IsSuccessful)
            {
                if (result.Message == Constants.NoRecordsFound)
                {
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);
                }
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status201Created, result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var loggedUser = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);

            if (!loggedUser.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, loggedUser.Message);

            }

            var result = await this.currencyService.GetAllAsync();
            if (!result.IsSuccessful)
            {
                if (result.Message == Constants.NoRecordsFound)
                {
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);
                }
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status200OK, result.Data);
        }

        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var loggedUser = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUser.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, loggedUser.Message);
            }

            var result = await this.currencyService.GetCurrencyByIdAsync(id);
            if (!result.IsSuccessful)
            {
                if (result.Message == Constants.NoRecordsFound)
                {
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);
                }
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status200OK, result.Data);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] CreateCurrencyDto currencyDto)
        {
            var loggedUser = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUser.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, loggedUser.Message);
            }
        
            var result = await this.currencyService.UpdateAsync(id, loggedUser.Data);
            if (!result.IsSuccessful)
            {
                if (result.Message == Constants.NoRecordsFound)
                {
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);
                }
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status200OK, result.Data);

        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var loggedUser = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUser.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUser.Message);
            }

            var result = await this.currencyService.DeleteAsync(id, loggedUser.Data);
            if (!result.IsSuccessful)
            {
                if (result.Message == Constants.NoRecordsFound)
                {
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);
                }

                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status200OK, result.Data);

        }
    }
}