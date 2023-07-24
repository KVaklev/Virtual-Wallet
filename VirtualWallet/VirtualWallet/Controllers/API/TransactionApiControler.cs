using AutoMapper;
using Business.Services.Contracts;
using Business.Dto;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using VirtualWallet.Helpers;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/transaction")]
    public class TransactionApiControler : Controller
    {
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;
        private readonly ITransactionService transactionService;
      //  private readonly IAcountService acountService;
        private readonly HelpersApi helpersApi;//todo-interface

        public TransactionApiControler(IMapper mapper, IAuthManager authManager, HelpersApi helpersApi )
        {
            this.mapper = mapper;
            this.authManager = authManager;
            this.transactionService = transactionService;
            this.helpersApi = helpersApi;
        }

        [HttpPost("")]
        public IActionResult Create([FromBody]CreateTransactionDto transactionDto)
        {
            var loggedUser = this.helpersApi.FindLoggedUser();

            var transaction = new Transaction();
           

            var createdTransaction=this.transactionService.Create(transaction, loggedUser);

            return StatusCode(StatusCodes.Status201Created, createdTransaction);
        }
    }
}
