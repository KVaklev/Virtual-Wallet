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
            try
            {
                var loggedUser = FindLoggedUser();
                var createdTransaction = this.transactionService.Create(transactionDto, loggedUser);
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
        public IActionResult Delete(int id)
        {
            try
            {
                var loggedUser = FindLoggedUser();
                var isDelete = this.transactionService.Delete(id, loggedUser);
                return StatusCode(StatusCodes.Status200OK, isDelete);
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
                var updateTransaction = this.transactionService.Update(id, loggedUser, transactionDto);
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
        public IActionResult GetById(int id)
        {
            try
            {
                var loggedUser = FindLoggedUser();
                var transactionDto = this.transactionService.GetById(id, loggedUser);
                
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

            private User FindLoggedUser()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = authManager.TryGetUserByUsername(loggedUsersUsername);
            return loggedUser;
        }
       
    }
}
