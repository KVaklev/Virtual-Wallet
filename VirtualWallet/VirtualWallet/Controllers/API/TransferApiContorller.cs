using AutoMapper;
using Business.Dto;
using Business.DTOs;
using Business.Exceptions;
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
        [HttpGet, Authorize] 
        public async Task<IActionResult> GetTransferAsync([FromQuery] TransferQueryParameters filterParameters)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var transfers = await this.transferService.FilterByAsync(filterParameters, loggedUser);
                List<GetTransferDto> transferDtos = transfers.Select(transfer => mapper.Map<GetTransferDto>(transfer)).ToList();

                return StatusCode(StatusCodes.Status200OK, transferDtos);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
        }

        [HttpGet("{id}"), Authorize]

        public async Task<IActionResult>GetTransferByIdAsync(int id)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var transferDto = await this.transferService.GetByIdAsync(id, loggedUser);

                return StatusCode(StatusCodes.Status200OK, transferDto);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTransferDto createTransferDto)

        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var newTransfer = await this.transferService.CreateAsync(createTransferDto, loggedUser);
                var newTransferDto = this.mapper.Map<GetTransferDto>(newTransfer);

                return StatusCode(StatusCodes.Status201Created, newTransferDto);

            }

            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }

            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
        }


        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] CreateTransferDto createTransferDto)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();

                var updateTransfer = await this.transferService.UpdateAsync(id, createTransferDto, loggedUser);
                
                var updateTransferDto = this.mapper.Map<GetTransferDto>(updateTransfer);
                return StatusCode(StatusCodes.Status200OK, updateTransferDto);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
        }

        [HttpDelete("{id}"), Authorize]

        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var loggeduser = await FindLoggedUserAsync();
                var isCancelled = await this.transferService.DeleteAsync(id, loggeduser);
                return StatusCode(StatusCodes.Status200OK);

            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
        }


        [HttpPut("{id}/execute"), Authorize]
        public async Task<IActionResult> ExecuteAsync(int id)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var isConfirmed= await this.transferService.ExecuteAsync(id, loggedUser);
                return StatusCode(StatusCodes.Status200OK, isConfirmed);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }

        }
        private async Task <User> FindLoggedUserAsync()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = await authManager.TryGetUserByUsernameAsync(loggedUsersUsername);
            return loggedUser;
        }

    }
}
