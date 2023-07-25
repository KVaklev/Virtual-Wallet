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
    public class TransactionApiControler : Controller
    {
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;
        private readonly ITransactionService transactionService;
        private readonly IAccountService accountService;
        private readonly IUserService userService;
        private readonly HelpersApi helpersApi;

        public TransactionApiControler(
            IMapper mapper, 
            IAuthManager authManager,
            ITransactionService transactionService,
            IAccountService accountService,
            IUserService userService,
            HelpersApi helpersApi
            )
        {
            this.mapper = mapper;
            this.authManager = authManager;
            this.transactionService = transactionService;
            this.accountService = accountService;
            this.userService = userService;
            this.helpersApi = helpersApi;
            
        }

        [HttpPost(""), Authorize]
        public IActionResult Create([FromBody]CreateTransactionDto transactionDto)
        {
            var loggedUser = this.helpersApi.FindLoggedUser();
            var recipientUser = this.userService
                .GetAll()
                .Where(u => u.Username == transactionDto.RecepiendUsername)
                .FirstOrDefault();

            var transaction = new Transaction();
            transaction.AccountSenderId = loggedUser.Id;
            transaction.AccountRecepientId = recipientUser.Id;
            transaction.Amount = transactionDto.Amount;
            

            var createdTransaction=this.transactionService.Create(transaction, loggedUser);

            return StatusCode(StatusCodes.Status201Created, createdTransaction);
        }
    }
}
