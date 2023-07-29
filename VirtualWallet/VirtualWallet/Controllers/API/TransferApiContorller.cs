using AutoMapper;
using Business.Dto;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Models;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;
using System.Text.Json;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/transfer")]
    public class TransferApiContorller : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;
        private readonly ITransferService transferService;
        private readonly IAccountService accountService;
        private readonly ICardService cardService;
                private readonly IUserService userService;
        private readonly ICurrencyService currencyService;

        public TransferApiContorller(
            IMapper mapper,
            IAuthManager authManager,
            ITransferService transferService,
            IAccountService accountService,
            ICardService cardService,
            IUserService userService,
            ICurrencyService currencyService
            )
        {
            this.mapper = mapper;
            this.authManager = authManager;
            this.transferService = transferService;
            this.accountService = accountService;
            this.userService = userService;
            this.currencyService = currencyService;

        }

        //[HttpGet, Authorize]

        //public IActionResult GetTransfers([FromQuery] TransferQueryParameters transferQueryParameters)
        //{
        //    try
        //    {
        //        List<Transfer> result = transferService.FilterBy(transferQueryParameters, user)

        //        List<TransferDto> transferDtos = new List<TransferDto>();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        //[HttpPost, Authorize]

        //public IActionResult Create([FromBody] TransferDto transferDto )
        //{

        //}

        private User FindLoggedUser()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = authManager.TryGetUserByUsername(loggedUsersUsername);
            return loggedUser;
        }




        //[HttpGet]
        //public IActionResult Withdraw(int id)
        //{

        //    var transfers = transferService.GetAll()
        //        .Where(u => u.Id == id)
        //                        .ToList();

        //    var json = JsonSerializer.Serialize(transfers);

        //    return Ok(json);

        //}

    }
}
