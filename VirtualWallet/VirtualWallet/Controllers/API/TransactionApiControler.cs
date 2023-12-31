﻿using Business.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Business.QueryParameters;
using Business.DTOs.Requests;
using Business.Services.Helpers;
using System.Security.Claims;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionApiControler : ControllerBase
    {
        private readonly ITransactionService transactionService;
        private readonly IUserService userService;

        public TransactionApiControler(
            ITransactionService transactionService,
            IUserService userService)
        {
            this.transactionService = transactionService;
            this.userService = userService;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTransactionDto transactionDto)
        {
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, loggedUserResult.Message);
            }

            var result = await this.transactionService.CreateOutTransactionAsync(transactionDto, loggedUserResult.Data);
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


        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, loggedUserResult.Message);
            }

            var result = await this.transactionService.DeleteAsync(id, loggedUserResult.Data);
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
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] CreateTransactionDto transactionDto)
        {
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, loggedUserResult.Message);
            }

            var result = await this.transactionService.UpdateAsync(id, loggedUserResult.Data, transactionDto);
            if (!result.IsSuccessful)
            {
                if (!result.IsSuccessful)
                if (loggedUserResult.Message == Constants.NoRecordsFound)
                {
                    if (result.Message == Constants.NoRecordsFound)
                    {
                        return StatusCode(StatusCodes.Status404NotFound, result.Message);
                    }
                    return BadRequest(result.Message);
                }
            }

            return StatusCode(StatusCodes.Status200OK, result.Data);

        }

        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, loggedUserResult.Message);
            }

            var result = await this.transactionService.GetByIdAsync(id, loggedUserResult.Data);
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

        [HttpGet, Authorize]
        public async Task<IActionResult> GetTransactionsAsync([FromQuery] TransactionQueryParameters filterParameters)
        {
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, loggedUserResult.Message);
            }

            var result = await this.transactionService.FilterByAsync(filterParameters, loggedUserResult.Data);
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

        [HttpPut("{id}/execute"), Authorize]
        public async Task<IActionResult> ExecuteAsync(int id)
        {
            var loggedUserResult = await userService.FindLoggedUserAsync(User.FindFirst(ClaimTypes.Name)?.Value!);
            if (!loggedUserResult.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, loggedUserResult.Message);
            }

            var result = await this.transactionService.ConfirmAsync(id, loggedUserResult.Data);
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


