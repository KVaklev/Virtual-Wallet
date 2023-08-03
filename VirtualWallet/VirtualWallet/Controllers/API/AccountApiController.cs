using Business.DTOs.Requests;
using Business.Exceptions;
using Business.Services.Contracts;
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

        public AccountApiController( 
            IUserService userService,
            IEmailService emailService,
            IAccountService accountService
            )
        {
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
                var loggedUser = await this.userService.LoginAsync(username, password);
                var token = await this.accountService.CreateApiTokenAsync(loggedUser);

                Response.Cookies.Append("Cookie_JWT", token.ToString(), new CookieOptions()
                {
                    HttpOnly = false,
                    SameSite = SameSiteMode.Strict
                });

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
              
               return await this.SendConfirmationEmailAsync(createdUser.Username);
            }
            catch (DuplicateEntityException e)
            {
                return StatusCode(StatusCodes.Status409Conflict, e.Message);
            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> SendConfirmationEmailAsync(string username)
        {
            var user = await this.userService.GetByUsernameAsync(username);
            var token = this.accountService.GenerateTokenAsync(user.Id);
            var confirmationLink = Url.Action("confirm-registration", "api", new { userId = user.Id, token = token.Result }, Request.Scheme);
            
            var message = await this.emailService.BuildEmailAsync(user, confirmationLink);
            
            await emailService.SendEMailAsync(message);

            return Ok("Confirmation email was sent successfully. Please check your inbox folder.");
        }


        [HttpGet("confirm-registration")]
        public async Task<IActionResult> ConfirmRegistrationAsync([FromQuery] int userId, [FromQuery] string token)
        {
           var isSuccessfullConfirmation = await this.accountService.ConfirmRegistrationAsync(userId, token);

            if (isSuccessfullConfirmation)
            {
                return Ok("Registration confirmed!");

            }
            return BadRequest("Your registration was not successfull.");
        }
    }
}
