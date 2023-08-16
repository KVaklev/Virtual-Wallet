using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.ViewModels;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Security.Claims;
using Business.Mappers;
using Business.Services.Helpers;

namespace VirtualWallet.Controllers.MVC
{
    [Authorize]
    public class TransferController : Controller
    {
        private readonly ITransferService transferService;
        private readonly IUserService userService;
        private readonly ICardService cardService;
        private readonly IExchangeRateService exchangeRateService;
        private readonly IMapper mapper;

        public TransferController(ITransferService transferService,
            IUserService userService,
            IExchangeRateService exchangeRateService,
            ICardService cardService,
            IMapper mapper)
        {
            this.transferService = transferService;
            this.userService = userService;
            this.exchangeRateService = exchangeRateService;
            this.cardService = cardService;
            this.mapper = mapper;
        }

        [HttpGet]

        public async Task<IActionResult> Index([FromQuery] TransferQueryParameters parameters)
        {
            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var result = await transferService.FilterByAsync(parameters, loggedUser.Data);

            var indexTransferViewModel = new IndexTransferViewModel();

            indexTransferViewModel.TransferQueryParameters = parameters;

            indexTransferViewModel.User = loggedUser.Data;

            if (!result.IsSuccessful)
            {
                if (result.Message == Constants.ModifyNoRecordsFound)
                {
                    this.ViewData["ErrorMessage"] = result.Message;

                    return View(indexTransferViewModel);
                }
                else
                {
                    this.ViewData["Controller"] = "Transfer";

                    return View("ErrorMessage", result.Message);
                }
            }

            indexTransferViewModel.TransferDtos = result.Data;
            return View(indexTransferViewModel);
        }



        [HttpGet]
        public async Task<IActionResult> Details([FromRoute] int id)
        {
            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var transferResult = await this.transferService.GetByIdAsync(id, loggedUser.Data);

            if (!transferResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "Transfer";

                return View("ErrorMessage", transferResult.Message);
            }

            var detailsTransferViewModel = new DetailsTransferViewModel();

            detailsTransferViewModel.GetTransferDto = transferResult.Data;

            detailsTransferViewModel.LoggedUser = loggedUser.Data;

            return this.View(detailsTransferViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var loggedUser = await FindLoggedUserAsync();
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var createTransferViewModel = new CreateTransferViewModel();
            var cardsResult = this.cardService.GetAll(loggedUser.Data);
            if (!cardsResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "Transfer";

                return View("ErrorMessage", cardsResult.Message);
            }

            var mappedCards = cardsResult.Data.Select(card => mapper.Map<GetCreatedCardDto>(card)).ToList();

            TempData["Cards"] = JsonSerializer.Serialize(mappedCards);

            return View("Create", createTransferViewModel);

        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateTransferViewModel transferDto)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(transferDto);
            }

            var loggedUser = await FindLoggedUserAsync();
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var transferResult = await this.transferService.CreateAsync(transferDto.CreateTransferDto, loggedUser.Data);

            if (!transferResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "Transfer";

                return View("ErrorMessage", transferResult.Message);
            }

            return this.RedirectToAction("Confirm", "Transfer", new { id = transferResult.Data.Id });

        }

        [HttpGet]
        public async Task<IActionResult> Update([FromRoute] int id)
        {
            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }
            var transferResult = await this.transferService.GetByIdAsync(id, loggedUser.Data);

            if (!transferResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "Transfer";

                return View("ErrorMessage", transferResult.Message);
            }

            var createTransferViewModel = new CreateTransferViewModel();

            createTransferViewModel.CreateTransferDto = await TransfersMapper.MapGetDtoToCreateDto(transferResult.Data);

            var cardsResult = this.cardService.GetAll(loggedUser.Data);

            if (!cardsResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "Transfer";

                return View("ErrorMessage", cardsResult.Message);
            }

            var mappedCards = cardsResult.Data.Select(card => mapper.Map<GetCreatedCardDto>(card)).ToList();

            TempData["Cards"] = JsonSerializer.Serialize(mappedCards);

            return View(createTransferViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateTransferDto transferDto)
        {
            if (!this.ModelState.IsValid)
            {
                return View(transferDto);
            }

            var loggedUser = await FindLoggedUserAsync();
            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }
            var transferResult = await this.transferService.UpdateAsync(id, transferDto, loggedUser.Data);

            if (!transferResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "Transaction";

                return View("ErrorMessage", transferResult.Message);
            }

            return this.RedirectToAction("Confirm", "Transfer", new { Username = loggedUser.Data.Username });

        }

        [HttpGet]

        public async Task<IActionResult> Delete([FromRoute] int id)
        {

            var loggedUserresult = await FindLoggedUserAsync();

            if (!loggedUserresult.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var transferResult = await this.transferService.GetByIdAsync(id, loggedUserresult.Data);

            if (!transferResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "Transfer";

                return View("ErrorMessage", transferResult.Message);
            }

            var result = await ExecuteTransferAsync(transferResult.Data);

            if (!result.IsSuccessful)
            {
                this.ViewData["Controller"] = "Transfer";

                return View("ErrorMessage", result.Message);
            }

            ConfirmTransferViewModel confirmTransferViewModel = result.Data;

            return this.View(confirmTransferViewModel);
        }

        [HttpPost]

        public async Task<IActionResult> Delete([FromRoute] int id,
            ConfirmTransferViewModel confirmTransferViewModel)
        {
            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var result = await this.transferService.DeleteAsync(id, loggedUser.Data);

            if (!result.IsSuccessful)
            {
                this.ViewData["Controller"] = "Transfer";

                return View("ErrorMessage", result.Message);
            }

            return this.View("SuccessfulCancellation");
        }


        [HttpGet]
        public async Task<IActionResult> Confirm([FromRoute] int id)
        {
            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var transferResult = await this.transferService.GetByIdAsync(id, loggedUser.Data);

            if (!transferResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "Transaction";

                return View("ErrorMessage", transferResult.Message);
            }

            var result = await ExecuteTransferAsync(transferResult.Data);

            if (!result.IsSuccessful)
            {
                this.ViewData["Controller"] = "Transfer";

                return View("ErrorMessage", result.Message);
            }

            ConfirmTransferViewModel confirmTransferViewModel = result.Data;

            return this.View(confirmTransferViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm([FromRoute] int id, ConfirmTransferViewModel confirmViewModel)
        {
            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }

            var transferResult = await this.transferService.GetByIdAsync(id, loggedUser.Data);

            if (!transferResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "Transfer";

                return View("ErrorMessage", transferResult.Message);
            }

            confirmViewModel.GetTransferDto = transferResult.Data;

            var userResult = await this.userService.GetLoggedUserByUsernameAsync(transferResult.Data.Username);

            if (!userResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "Transfer";

                return View("ErrorMessage", userResult.Message);
            }

            confirmViewModel.UserDetails = userResult.Data;

            var result = await this.transferService.ConfirmAsync(id, loggedUser.Data);

            if (!result.IsSuccessful)
            {
                this.ViewData["Controller"] = "Transfer";

                return View("ErrorMessage", result.Message);
            }

            return RedirectToAction("SuccessfulConfirmation", "Transfer", new { id = transferResult.Data.Id });

        }

        [HttpGet]
        public async Task<IActionResult> SuccessfulConfirmation([FromRoute] int id)
        {
            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return this.RedirectToAction("Login", "Account");
            }
            var transferResult = await this.transferService.GetByIdAsync(id, loggedUser.Data);

            if (!transferResult.IsSuccessful)
            {
                this.ViewData["Controller"] = "Transfer";

                return View("ErrorMessage", transferResult.Message);
            }
            return View(transferResult.Data);

        }

        private async Task<Response<ConfirmTransferViewModel>> ExecuteTransferAsync(GetTransferDto transferDto)
        {
            var result = new Response<ConfirmTransferViewModel>();

            ConfirmTransferViewModel transferViewModel = new ConfirmTransferViewModel();

            transferViewModel.GetTransferDto = transferDto;

            var user = await this.userService.GetLoggedUserByUsernameAsync(transferDto.Username);

            if (!user.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = user.Message;
                return result;
            }

            transferViewModel.UserDetails = user.Data;

            var exchangeAmount = await this.exchangeRateService.ExchangeAsync(
                transferDto.Amount,
                transferDto.Card.Currency.CurrencyCode,
                user.Data.Account.Currency.CurrencyCode);

            if (!exchangeAmount.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = exchangeAmount.Message;
                return result;
            }

            transferViewModel.GetTransferDto.Amount = exchangeAmount.Data;

            result.Data = transferViewModel;

            return result;
        }

        private async Task<Response<User>> FindLoggedUserAsync()
        {
            var result = new Response<User>();
            var loggedUsersUsername = User.FindFirst(ClaimTypes.Name);
            if (loggedUsersUsername == null)
            {
                result.IsSuccessful = false;
                return result;
            }
            var loggedUserResult = await this.userService.GetLoggedUserByUsernameAsync(loggedUsersUsername.Value);
            if (loggedUserResult == null)
            {
                result.IsSuccessful = false;
                return result;
            }
            return loggedUserResult;
        }

    }
}
