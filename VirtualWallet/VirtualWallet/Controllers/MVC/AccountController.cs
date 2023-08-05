using Business.DTOs.Requests;
using Business.Services.Contracts;
using Business.Services.Helpers;
using Business.ViewModels;
using DataAccess.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.MVC
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IUserService userService;
        private readonly IAccountService accountService;
        private readonly IUserRepository userRepository;
        private readonly IEmailService emailService;

        public AccountController(
            IUserService userService,
            IAccountService accountService,
            IUserRepository userRepository,
            IEmailService emailService)
        {
            this.userService = userService;
            this.accountService = accountService;
            this.userRepository = userRepository;
            this.emailService = emailService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var loginViewModel = new LoginUserModel();

            return this.View(loginViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserModel loginUserModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(loginUserModel);
            }

            var loggedUser = await this.userService.LoginAsync(loginUserModel.Username, loginUserModel.Password);
           
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

            result.Message = Constants.SuccessfullLoggedInMessage;

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            if (Request.Cookies["Cookie_JWT"] != null)
            {
                Response.Cookies.Delete("Cookie_JWT");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            var createUserModel = new CreateUserModel();

            return this.View(createUserModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(CreateUserModel createUserModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(createUserModel);
            }

            var result = await this.userService.CreateAsync(createUserModel);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            return await this.SendConfirmationEmailAsync(result.Data.Username);
        }

        public async Task<IActionResult> SendConfirmationEmailAsync(string username)
        {
            var user = await this.userRepository.GetByUsernameAsync(username);
            var generatedToken = this.accountService.GenerateTokenAsync(user.Id);

            if (!generatedToken.IsCompletedSuccessfully)
            {
                return RedirectToAction("Register", "Account");
            }
            var confirmationLink = Url.Action("confirm-registration", "api", new { userId = user.Id, token = generatedToken.Result }, Request.Scheme);

            var message = await this.emailService.BuildEmailAsync(user, confirmationLink);

            await emailService.SendEMailAsync(message);

            return RedirectToAction("RegistrationSuccessful", "Account");
        }

        public IActionResult RegistrationSuccessful()
        {
			return this.View("RegistrationSuccessful");
		}

        [HttpGet("confirm-registration")]
        public async Task<IActionResult> ConfirmRegistrationAsync([FromQuery] int userId, [FromQuery] string token)
        {
			if (!ModelState.IsValid)
			{
				return RedirectToAction("Index", "Home");
			}

			var result = await this.accountService.ConfirmRegistrationAsync(userId, token);

            if (!result.IsSuccessful)
            {
				return RedirectToAction("Index", "Home");

			}
            return RedirectToAction("Login", "Account");
        }
    }
}
