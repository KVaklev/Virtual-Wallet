using AutoMapper;
using Business.Dto;
using Business.Exceptions;
using Business.Services.Additional;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Presentation.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api")]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthManager authManager;
        private readonly IUserService userService;
        private readonly IEmailService emailService;
        private readonly IAccountService accountService;

        public AuthApiController(
            IAuthManager authManager, 
            IUserService userService,
            IEmailService emailService,
            IAccountService accountService
            )
        {
            this.authManager = authManager;
            this.userService = userService;
            this.emailService = emailService;
            this.accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(string username, string password)
        {
            try
            {
                this.authManager.CheckForNullEntry(username, password);
                var loggedUser = await this.authManager.TryGetUserByUsernameAsync(username);
                var authenticatedUser = await this.authManager.AuthenticateAsync(loggedUser, username, password);

                string token = await CreateApiTokenAsync(loggedUser);

                return Ok("Logged in successfully. Token: " + token);
            }
            catch (ArgumentException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
            catch
            {
                return BadRequest("An error occurred in generating the token");
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                var createdUser = await this.userService.CreateAsync(createUserDto);
                await this.SendConfirmationEmailAsync(createdUser.Username);
                
               return StatusCode(StatusCodes.Status201Created, createdUser);
            }
            catch (DuplicateEntityException e)
            {
                return StatusCode(StatusCodes.Status409Conflict, e.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<IActionResult> SendConfirmationEmailAsync(string username)
        {
            var user = await this.userService.GetByUsernameAsync(username);

            var token = this.accountService.GenerateTokenAsync(user.Id);
            var confirmationLink = Url.Action(nameof(ConfirmRegistration), "AuthApi",
                                   new { userId = user.Id, token = token.Result }, Request.Scheme);

            var message = this.emailService.BuildEmail(user, confirmationLink);
            emailService.SendEMail(message);

            return Ok("Confirmation email was sent successfully. Please check your inbox folder.");
        }


        [HttpGet("confirm-registration")]
        public async Task<IActionResult> ConfirmRegistration([FromQuery] int userId, [FromQuery] string token)
        {
            await this.accountService.ConfirmRegistrationAsync(userId, token);
            return Ok("Registration confirmed!");
        }



        private async Task<string> CreateApiTokenAsync(User loggedUser)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("This is my secret testing key"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                    issuer: "VirtualWallet",
                    audience: "Where is that audience",
                    claims: new[] {
                new Claim("LoggedUserId", loggedUser.Id.ToString()),
                new Claim("Username", loggedUser.Username),
                new Claim("IsAdmin", loggedUser.IsAdmin.ToString()),
                new Claim("UsersAccountId", loggedUser.Account.Id.ToString()),//null check
                new Claim(JwtRegisteredClaimNames.Email, loggedUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        },
            expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: signinCredentials
                );
            string token = new JwtSecurityTokenHandler()

            .WriteToken(jwtSecurityToken);
            Response.Cookies.Append("Cookie_JWT", token);
            return await Task.FromResult(token);
        }
    }
}
