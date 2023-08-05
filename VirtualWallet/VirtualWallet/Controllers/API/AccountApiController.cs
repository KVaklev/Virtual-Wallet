﻿using Business.DTOs.Requests;
using Business.Exceptions;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api")]
    public class AccountApiController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IEmailService emailService;
        private readonly IAccountService accountService;
        private readonly IUserRepository userRepository;

        public AccountApiController( 
            IUserService userService,
            IEmailService emailService,
            IAccountService accountService,
            IUserRepository userRepository
            )
        {
            this.userService = userService;
            this.emailService = emailService;
            this.accountService = accountService;
            this.userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(string username, string password)
        {

            var loggedUser = await this.userService.LoginAsync(username, password);

            if (!loggedUser.IsSuccessful)
            {
                return BadRequest(loggedUser.Message);
            }

            var result = await this.accountService.CreateApiTokenAsync(loggedUser.Data);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            Response.Cookies.Append("Cookie_JWT", result.Data.ToString(), new CookieOptions()
            {
                HttpOnly = false,
                SameSite = SameSiteMode.Strict
            });

            result.Message = Constants.SuccessfullTokenMessage;

            return Ok(result.Message + result.Data.ToString());
   
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] CreateUserModel createUserDto)
        {
   
           var result = await this.userService.CreateAsync(createUserDto);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

           return await this.SendConfirmationEmailAsync(result.Data.Username);
            
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> SendConfirmationEmailAsync(string username)
        {
            var user = await this.userRepository.GetByUsernameAsync(username);
            var token = this.accountService.GenerateTokenAsync(user.Id);
            var confirmationLink = Url.Action("confirm-registration", "api", new { userId = user.Id, token = token.Result }, Request.Scheme);
            
            var message = await this.emailService.BuildEmailAsync(user, confirmationLink);
            
            await emailService.SendEMailAsync(message);

            return Ok(Constants.SuccessfullConfirmationEmailSentMessage);
        }


        [HttpGet("confirm-registration")]
        public async Task<IActionResult> ConfirmRegistrationAsync([FromQuery] int userId, [FromQuery] string token)
        {
           var result = await this.accountService.ConfirmRegistrationAsync(userId, token);

            if (!result.IsSuccessful)
            {
                return BadRequest(Constants.NotSuccessfullRegistrationMessage);

            }
            return Ok(result.Message);
        }
    }
}
