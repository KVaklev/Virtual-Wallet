﻿using AutoMapper;
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
        private readonly IAuthManager authManager;

        public CurrencyApiController(
            ICurrencyService currencyService,
            IMapper mapper,
            IUserService userService,
            IAuthManager authManager

            )
        {
            this.currencyService = currencyService;
            this.mapper = mapper;
            this.userService = userService;
            this.authManager = authManager;
        }

        [HttpPost, Authorize]
        public IActionResult Create([FromBody] CurrencyDto currencyDto)
        {
            try
            {
                var loggedUser = FindLoggedUser();
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

        [HttpGet]
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

        [HttpPut("{id}"), Authorize]
        public IActionResult Update([FromRoute] int id, [FromBody] CurrencyDto currencyDto)
        {
            try
            {
                var loggedUser = FindLoggedUser();
                var currency = this.mapper.Map<Currency>(currencyDto);
                var updateCurency = this.currencyService.Update(id, currency, loggedUser);
                var currencyUpdateDto = this.mapper.Map<CurrencyDto>(updateCurency);
                return StatusCode(StatusCodes.Status200OK, currencyUpdateDto);
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

        [HttpDelete("{id}"), Authorize]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                var loggedUser = FindLoggedUser();
                var isDeleted = this.currencyService.Delete(id, loggedUser);
                return StatusCode(StatusCodes.Status200OK, isDeleted);
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
        private User FindLoggedUser()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = authManager.TryGetUserByUsername(loggedUsersUsername);
            return loggedUser;
        }
    }
}