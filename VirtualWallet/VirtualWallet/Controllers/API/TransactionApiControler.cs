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
        public IActionResult Create([FromBody] CreateTransactionDto transactionDto)
        {
           // Todo map method

            var loggedUser = FindLoggedUser();
            var transaction = MapDtoТоTransaction(transactionDto);
            var createdTransaction = this.transactionService.Create(transaction, loggedUser);
            var createdTransactionDto = MapTransactionToDto(createdTransaction, loggedUser);

            return StatusCode(StatusCodes.Status201Created, createdTransactionDto);
        }

        [HttpDelete("{id}"), Authorize]
        public IActionResult Delete(int id)
        {
            try
            {
                var loggedUser = FindLoggedUser();
                var isDeleted = this.transactionService.Delete(id, loggedUser);
                return StatusCode(StatusCodes.Status200OK);
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
        public IActionResult Update(int id, [FromBody] CreateTransactionDto transactionDto)
        {
            try
            {
                var loggedUser = FindLoggedUser();
                var transaction = MapDtoТоTransaction(transactionDto);
                var updateTransaction = this.transactionService.Update(id, loggedUser, transaction);
                var updateTransactionDto = MapTransactionToDto(updateTransaction, loggedUser);
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
        public IActionResult GetById(int id)
        {
            try
            {
                var loggedUser = FindLoggedUser();
                var transaction = this.transactionService.GetById(id, loggedUser);
                var transactionDto = MapTransactionToDto(transaction, loggedUser);
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
        public IActionResult GetTransactions([FromQuery] TransactionQueryParameters filterParameters)
        {
            try
            {
                var loggedUser = FindLoggedUser();
                var transacrions = this.transactionService.FilterBy(filterParameters, loggedUser);
                List<GetTransactionDto> transactionDtos = transacrions
                    .Select(transaction => MapTransactionToDto(transaction, loggedUser))
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
        public IActionResult Execute(int id)
        {
            try
            {
                var loggedUser = FindLoggedUser();
                var isExecute = this.transactionService.Execute(id, loggedUser);
                return StatusCode(StatusCodes.Status200OK, isExecute);
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

        private GetTransactionDto MapTransactionToDto(Transaction transaction, User loggedUser)
        {
            var accountRecipient = this.accountService.GetById(transaction.AccountRecepientId, loggedUser);
            var recipient = this.userService.GetByIdAsync((int)accountRecipient.UserId);
            var currency = this.currencyService.GetByIdAsync(transaction.CurrencyId);
            var getTransactionDto = new GetTransactionDto();
            getTransactionDto.RecipientUsername = recipient.Username;
            getTransactionDto.Amount = transaction.Amount;
            getTransactionDto.Date = transaction.Date;
            getTransactionDto.Direction = transaction.Direction.ToString();
            getTransactionDto.Abbreviation = transaction.Currency.Abbreviation;
            return getTransactionDto;

        }

        private Transaction MapDtoТоTransaction(CreateTransactionDto transactionDto)
        {
            var loggedUser = FindLoggedUser();
            var userRecipient = this.userService.GetByUsernameAsync(transactionDto.RecepientUsername);
            var currency = this.currencyService.GetByАbbreviationAsync(transactionDto.Currency);

            var transaction = new Transaction();
            transaction.AccountSenderId = loggedUser.Id;
            transaction.AccountRecepientId = (int)userRecipient.AccountId;
            transaction.Amount = transactionDto.Amount;
            transaction.CurrencyId = currency.Id;

            return transaction;
        }

        private async Task<User> FindLoggedUserAsync()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = await authManager.TryGetUserByUsernameAsync(loggedUsersUsername);
            return loggedUser;
        }
    }
}
