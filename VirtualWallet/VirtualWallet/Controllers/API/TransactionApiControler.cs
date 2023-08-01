using AutoMapper;
using Business.Services.Contracts;
using Business.Dto;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using Microsoft.AspNetCore.Authorization;
using Business.Exceptions;
using Business.DTOs;
using Business.QueryParameters;
using DataAccess.Repositories.Contracts;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionApiControler : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;
        private readonly ITransactionService transactionService;
        private readonly IAccountService accountService;
        private readonly IUserService userService;
        private readonly ICurrencyService currencyService;
        
        public TransactionApiControler(
            IMapper mapper,
            IAuthManager authManager,
            ITransactionService transactionService,
            IAccountService accountService,
            IUserService userService,
            ICurrencyService currencyService
            )
        {
            this.mapper = mapper;
            this.authManager = authManager;
            this.transactionService = transactionService;
            this.accountService = accountService;
            this.userService = userService;
            this.currencyService = currencyService;
            
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTransactionDto transactionDto)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var createdTransaction = await this.transactionService.CreateAsync(transactionDto, loggedUser);
                var createdTransactionDto = this.mapper.Map<GetTransactionDto>(createdTransaction);

                return StatusCode(StatusCodes.Status201Created, createdTransactionDto);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }

        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var isDeleted = await this.transactionService.DeleteAsync(id, loggedUser);
                return StatusCode(StatusCodes.Status200OK, isDeleted);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] CreateTransactionDto transactionDto)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var updateTransaction = await this.transactionService.UpdateAsync(id, loggedUser, transactionDto);
                var updateTransactionDto = this.mapper.Map<GetTransactionDto>(updateTransaction);
                return StatusCode(StatusCodes.Status200OK, updateTransactionDto);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
        }

        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var transactionDto = await this.transactionService.GetByIdAsync(id, loggedUser);

                return StatusCode(StatusCodes.Status200OK, transactionDto);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }

        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetTransactionsAsync([FromQuery] TransactionQueryParameters filterParameters)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var transacrions = await this.transactionService.FilterByAsync(filterParameters, loggedUser);
                List<GetTransactionDto> transactionDtos = transacrions
                    .Select(transaction => mapper.Map<GetTransactionDto>(transaction))
                    .ToList();

                return StatusCode(StatusCodes.Status200OK, transactionDtos);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
        }

        [HttpPut("{id}/execute"), Authorize]
        public async Task<IActionResult> ExecuteAsync(int id)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                
                var isExecuted = await this.transactionService.ExecuteAsync(id, loggedUser);
                return StatusCode(StatusCodes.Status200OK, isExecuted);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
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
    

