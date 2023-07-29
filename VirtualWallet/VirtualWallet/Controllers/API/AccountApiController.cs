using AutoMapper;
using Business.Dto;
using Business.DTOs;
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

        [HttpGet(""), Authorize]

        public ActionResult GetAllAcounts()
        {
            //var loggedUser = FindLoggedUser();

            var result = accountService.GetAll();

            List<GetAccountDto> accountDtos = result.Select(account => mapper.Map<GetAccountDto>(account)).ToList();

            return Ok(accountDtos);

        }

        //[HttpGet("{id}"), Authorize]

        //public async Task<ActionResult<IEnumerable<AccountDto>>> GetAllAcounts()
        //{
        //    var loggedUser = FindLoggedUser();

        //    var result = (await accountService.GetAll()).ToList();

        //    List<AccountDto> accountDtos = result.Select(account => mapper.Map<AccountDto>(account)).ToList();

        //    return Ok(accountDtos);

        //}

        //[HttpPost("")]
        //public  async Task<ActionResult> 

        //[HttpGet("getbyid")]

        [HttpPost(""), Authorize]
        public async Task<ActionResult> Create([FromBody] CreateAccountDto createAccountDto)

        {
            var loggedUser = FindLoggedUser();

            Account newAccount = await accountService.CreateAsync(createAccountDto, loggedUser);

            return Ok(newAccount);

        }

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
