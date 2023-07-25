using AutoMapper;
using Business.Dto;
using Business.Exceptions;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.Helpers;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/currency")]
    public class CurrencyApiController : ControllerBase
    {
        private readonly ICurrencyService currencyService;
        private readonly IMapper mapper;
        private readonly IUserService userService;
        private readonly HelpersApi helpersApi;

        public CurrencyApiController(
            ICurrencyService currencyService,
            IMapper mapper,
            IUserService userService,
            HelpersApi helpersApi)
        {
            this.currencyService = currencyService;
            this.mapper = mapper;
            this.userService = userService;
            this.helpersApi = helpersApi;
        }
        [HttpPost(""), Authorize]
        public IActionResult Create([FromBody] CurrencyDto currencyDto)
        {
            try
            {
                var loggUser = this.helpersApi.FindLoggedUser();
                var currency = this.mapper.Map<Currency>(currencyDto);
                this.currencyService.Create(currency, loggUser);
                return StatusCode(StatusCodes.Status200OK, currencyDto);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
            catch (UnauthorizedOperationException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
            }
        }

        [HttpGet("")]
        public IActionResult GetAll()
        {
            try
            {
                var currencies = this.currencyService.GetAll();
                return StatusCode(StatusCodes.Status200OK, currencies);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
        }
    }
}
