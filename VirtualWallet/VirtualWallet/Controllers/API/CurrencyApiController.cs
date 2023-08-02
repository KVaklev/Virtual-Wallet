using AutoMapper;
using Business.Exceptions;
using Business.DTOs.Responses;
using Business.DTOs.Requests;
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
                IQueryable<Currency> currencies = this.currencyService.GetAll();
                List<CurrencyDto> currenciesDto = currencies
                    .Select(currency => mapper.Map<CurrencyDto>(currency))
                    .ToList();

                return StatusCode(StatusCodes.Status200OK, currenciesDto);
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
                var currencyDto = this.mapper.Map<CurrencyDto>(currency);

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
                var currency = this.mapper.Map<Currency>(currencyDto);
                var updateCurency = await this.currencyService.UpdateAsync(id, currency, loggedUser);
                var currencyUpdateDto = this.mapper.Map<CurrencyDto>(updateCurency);

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