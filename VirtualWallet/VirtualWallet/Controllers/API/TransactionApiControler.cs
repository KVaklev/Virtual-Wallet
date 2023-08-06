using Business.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Business.Exceptions;
using Business.QueryParameters;
using Business.DTOs.Requests;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Business.Services.Helpers;

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
            IUserService userService
            )
        {
            this.transactionService = transactionService; 
            this.userService = userService;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTransactionDto transactionDto)
        {

            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                    return StatusCode(StatusCodes.Status404NotFound, loggedUserResult.Message);
            }
            var result = await this.transactionService.CreateOutTransactionAsync(transactionDto, loggedUserResult.Data);

            if (!result.IsSuccessful)
            {
                if (loggedUserResult.Message == Constants.NoFoundResulte)
                {
                    return StatusCode(StatusCodes.Status404NotFound, loggedUserResult.Message);
                }
                else 
                { 
                    return BadRequest(result.Message);
                }
            }
           
            return StatusCode(StatusCodes.Status201Created, result.Data);
        }


        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResult.Message);
            }
            var result = await this.transactionService.DeleteAsync(id, loggedUserResult.Data);
            if (!result.IsSuccessful)
            {
                if (loggedUserResult.Message == Constants.NoFoundResulte)
                {
                    return StatusCode(StatusCodes.Status404NotFound, loggedUserResult.Message);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            return StatusCode(StatusCodes.Status200OK, result.Message);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] CreateTransactionDto transactionDto)
        {
                var loggedUserResult = await FindLoggedUserAsync();
                if (!loggedUserResult.IsSuccessful)
                {
                    return StatusCode(StatusCodes.Status404NotFound, loggedUserResult.Message);
                }
                var result = await this.transactionService.UpdateAsync(id, loggedUserResult.Data, transactionDto);
                if (!result.IsSuccessful)
                {
                if (loggedUserResult.Message == Constants.NoFoundResulte)
                {
                    return StatusCode(StatusCodes.Status404NotFound, loggedUserResult.Message);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
                return StatusCode(StatusCodes.Status200OK, result.Data);
            
        }

        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResult.Message);
            }
            var result = await this.transactionService.GetByIdAsync(id, loggedUserResult.Data);
                if (!result.IsSuccessful)
                {
                if (loggedUserResult.Message == Constants.NoFoundResulte)
                {
                    return StatusCode(StatusCodes.Status404NotFound, loggedUserResult.Message);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
                return StatusCode(StatusCodes.Status200OK, result.Data);
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetTransactionsAsync([FromQuery] TransactionQueryParameters filterParameters)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResult.Message);
            }
            var result = await this.transactionService.FilterByAsync(filterParameters, loggedUserResult.Data);
                
                return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpPut("{id}/execute"), Authorize]
        public async Task<IActionResult> ExecuteAsync(int id)
        {
            var loggedUserResult = await FindLoggedUserAsync();
            if (!loggedUserResult.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUserResult.Message);
            }
            var result = await this.transactionService.ExecuteAsync(id, loggedUserResult.Data);
                if (!result.IsSuccessful)
                {
                if (loggedUserResult.Message == Constants.NoFoundResulte)
                {
                    return StatusCode(StatusCodes.Status404NotFound, loggedUserResult.Message);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
                return StatusCode(StatusCodes.Status200OK, result);
        }
        private async Task<Response<User>> FindLoggedUserAsync()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUserResult = await this.userService.GetLoggedUserByUsernameAsync(loggedUsersUsername);
            return loggedUserResult;
        }
    }
}  
    

