using AutoMapper;
using Business.DTOs;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/transfers")]
    public class TransferApiContorller : ControllerBase
    {
        private readonly ITransferService transferService;
        private readonly IUserService userService;


        public TransferApiContorller(
            ITransferService transferService,
            IUserService userService
            )
        {

            this.transferService = transferService;
            this.userService = userService;

        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTransferDto createTransferDto)

        {
            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUser.Message);
            }

            var result = await this.transferService.CreateAsync(createTransferDto, loggedUser.Data);

            if (!result.IsSuccessful)
            {
                if (result.Message == Constants.NoFoundResulte)
                {
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);
                }
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status201Created, result.Data);

        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetTransferAsync([FromQuery] TransferQueryParameters filterParameters)
        {
            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUser.Message);
            }

            var result = await this.transferService.FilterByAsync(filterParameters, loggedUser.Data);

            return StatusCode(StatusCodes.Status200OK, result.Data);

        }

        [HttpGet("{id}"), Authorize]

        public async Task<IActionResult> GetTransferByIdAsync(int id)
        {

            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUser.Message);
            }

            var result = await this.transferService.GetByIdAsync(id, loggedUser.Data);

            if (!result.IsSuccessful)
            {
                if (result.Message == Constants.NoFoundResulte)
                {
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }


        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateTransferDto updateTransferDto)
        {
            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUser.Message);
            }

            var result = await this.transferService.UpdateAsync(id, updateTransferDto, loggedUser.Data);

            if (!result.IsSuccessful)
            {
                if (result.Message == Constants.NoFoundResulte)
                {
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);
                }
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status200OK, result.Data);

        }

        [HttpDelete("{id}"), Authorize]

        public async Task<IActionResult> DeleteAsync(int id)
        {
            var loggeduser = await FindLoggedUserAsync();

            if (!loggeduser.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggeduser.Message);
            }

            var result = await this.transferService.DeleteAsync(id, loggeduser.Data);

            if (!result.IsSuccessful)
            {
                if (result.Message == Constants.NoFoundResulte)
                {
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);
                }
                return BadRequest(result.Message);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);

        }

        [HttpPut("{id}/execute"), Authorize]
        public async Task<IActionResult> ExecuteAsync(int id)
        {
            var loggedUser = await FindLoggedUserAsync();

            if (!loggedUser.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status404NotFound, loggedUser.Message);
            }

            var result = await this.transferService.ExecuteAsync(id, loggedUser.Data);

            if (!result.IsSuccessful)
            {
                if (result.Message == Constants.NoFoundResulte)
                {
                    return StatusCode(StatusCodes.Status404NotFound, result.Message);
                }
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status200OK, result.Data);
        }
        private async Task<Response<User>> FindLoggedUserAsync()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = await this.userService.GetLoggedUserByUsernameAsync(loggedUsersUsername);
            return loggedUser;
        }
    }
}
