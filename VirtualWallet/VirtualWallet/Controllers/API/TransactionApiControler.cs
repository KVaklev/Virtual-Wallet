using AutoMapper;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using Microsoft.AspNetCore.Authorization;
using Business.Exceptions;
using Business.QueryParameters;
using Business.DTOs.Requests;


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

            var loggedUser = await FindLoggedUserAsync();
            var result = await this.transactionService.CreateOutTransactionAsync(transactionDto, loggedUser);

            if (!result.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, result.Message);
            }
           
                return StatusCode(StatusCodes.Status201Created, result.Data);
        }


        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var result = await this.transactionService.DeleteAsync(id, loggedUser);
                if (!result.IsSuccessful)
                {
                    return BadRequest(result.Message);
                }
                return StatusCode(StatusCodes.Status200OK, result.Message);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] CreateTransactionDto transactionDto)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var result = await this.transactionService.UpdateAsync(id, loggedUser, transactionDto);
                if (!result.IsSuccessful)
                {
                    return BadRequest(result.Message);
                }
                return StatusCode(StatusCodes.Status200OK, result.Data);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var result = await this.transactionService.GetByIdAsync(id, loggedUser);
                if (!result.IsSuccessful)
                {
                    return BadRequest(result.Message);
                }
                return StatusCode(StatusCodes.Status200OK, result.Data);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetTransactionsAsync([FromQuery] TransactionQueryParameters filterParameters)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var result = await this.transactionService.FilterByAsync(filterParameters, loggedUser);
                
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpPut("{id}/execute"), Authorize]
        public async Task<IActionResult> ExecuteAsync(int id)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var result = await this.transactionService.ExecuteAsync(id, loggedUser);
                if (!result.IsSuccessful)
                {
                    return BadRequest(result.Message);
                }
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }
            private async Task<User> FindLoggedUserAsync()
            {
                var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
                var loggedUser = await this.userService.GetByUsernameAsync(loggedUsersUsername);
                return loggedUser;
            }
   }
}  
    

