using AutoMapper;
using Business.Dto;
using Business.Exceptions;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using VirtualWallet.Helpers;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/currencies")]
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
        [HttpPost(), Authorize]
        public IActionResult Create([FromBody] CurrencyDto currencyDto)
        {
            try
            {
                var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
                var loggedUser = this.userService.GetByUsername(loggedUsersUsername);
                var currency = this.mapper.Map<Currency>(currencyDto);
                this.currencyService.Create(currency, loggedUser);
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
                List<Currency> currencies = this.currencyService.GetAll();
                List<CurrencyDto> currenciesDto = currencies
                    .Select(currency => mapper.Map<CurrencyDto>(currency)).ToList();
                return StatusCode(StatusCodes.Status200OK, currenciesDto);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            try
            {
                var currency = this.currencyService.GetById(id);
                var currencyDto = this.mapper.Map<CurrencyDto>(currency);
                return StatusCode(StatusCodes.Status200OK, currencyDto);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
        }
    }
}
