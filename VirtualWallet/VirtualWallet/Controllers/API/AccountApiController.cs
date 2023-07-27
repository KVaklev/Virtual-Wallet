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
    [Route("api/accounts")]
    public class AccountApiController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;
        private readonly IAccountService accountService;


        public AccountApiController(IMapper mapper, IAuthManager authManager, IAccountService accountService)
        {
            this.mapper = mapper;
            this.authManager = authManager;
            this.accountService = accountService;
        }

        [HttpGet, Authorize]
        public IActionResult GatAllAcounts()
        {
            var loggedUser = FindLoggedUser();

            var result = accountService.GetAll().ToList();

            List<AccountDto> accountDtos = result.Select(account => mapper.Map<AccountDto>(account)).ToList();

            return Ok(accountDtos);

        }

        //[HttpPost, Authorize]
        //public IActionResult Create([FromBody] AccountDto accountDto)
        //{

        //}

        private User FindLoggedUser()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = authManager.TryGetUserByUsername(loggedUsersUsername);
            return loggedUser;
        }




        //[HttpGet("{id}")]
        //public IActionResult GetAccountById(int id) { }

        //[HttpPost]
        //public IActionResult CreateAccount(AccountDto accountDto)
    }
}
