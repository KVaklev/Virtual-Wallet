using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
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
    [Route("api/transfers")]
    public class TransferApiContorller : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ITransferService transferService;
        private readonly IUserService userService;

        public TransferApiContorller(
            IMapper mapper,
            ITransferService transferService,
            IUserService userService
            )
        {
            this.mapper = mapper;
            this.transferService = transferService;
            this.userService = userService;
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
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateTransferDto updateTransferDto)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();

                var updateTransfer = await this.transferService.UpdateAsync(id, updateTransferDto, loggedUser);
                
                var updatedTransferDto = this.mapper.Map<UpdateTransferDto>(updateTransfer);
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
            var loggedUser = await this.userService.GetByUsernameAsync(loggedUsersUsername);
            return loggedUser;
        }

    }
}
