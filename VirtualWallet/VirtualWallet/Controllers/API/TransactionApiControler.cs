using AutoMapper;
using Business.Services.Contracts;
using Business.Dto;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using VirtualWallet.Helpers;
using Microsoft.AspNetCore.Authorization;

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
        public IActionResult Create([FromBody]CreateTransactionDto transactionDto)
        {
            //Todo map method
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = this.userService.GetByUsername(loggedUsersUsername);
            var userRecipient = this.userService.GetByUsername(transactionDto.RecepientUsername);
            var currency = this.currencyService.GetByАbbreviation(transactionDto.Currency);

            var transaction = new Transaction();
            transaction.AccountSenderId = loggedUser.Id;
            transaction.AccountRecepientId = (int)userRecipient.AccountId;
            transaction.Amount = transactionDto.Amount;
            transaction.CurrencyId = currency.Id;
            

            var createdTransaction=this.transactionService.Create(transaction, loggedUser);

            return StatusCode(StatusCodes.Status201Created, createdTransaction);
        }
    }
}
