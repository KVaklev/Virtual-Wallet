using AutoMapper;
using Business.DTOs;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
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
        private readonly IUserRepository userRepository;

        public TransferApiContorller(
            ITransferService transferService,
            IUserService userService,
            IUserRepository userRepository
            )
        {

            this.transferService = transferService;
            this.userService = userService;
            this.userRepository = userRepository;
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTransferDto createTransferDto)

        {
            var loggedUser = await FindLoggedUserAsync();

            var result = await this.transferService.CreateAsync(createTransferDto, loggedUser);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status201Created, result.Data);

        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetTransferAsync([FromQuery] TransferQueryParameters filterParameters)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();

                var result = await this.transferService.FilterByAsync(filterParameters, loggedUser);

                return StatusCode(StatusCodes.Status200OK, result.Data);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpGet("{id}"), Authorize]

        public async Task<IActionResult> GetTransferByIdAsync(int id)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var result = await this.transferService.GetByIdAsync(id, loggedUser);

                if (!result.IsSuccessful)
                {
                    return BadRequest(result.Message);
                }
                return StatusCode(StatusCodes.Status200OK, result.Data);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }

        }


        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateTransferDto updateTransferDto)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();

                var result = await this.transferService.UpdateAsync(id, updateTransferDto, loggedUser);

                if (!result.IsSuccessful)
                {
                    return BadRequest(result.Message);
                }
                return StatusCode(StatusCodes.Status200OK, result.Data);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }

        }

        [HttpDelete("{id}"), Authorize]

        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var loggeduser = await FindLoggedUserAsync();
                var result = await this.transferService.DeleteAsync(id, loggeduser);
                if (!result.IsSuccessful)
                {
                    return BadRequest(result?.Message);
                }
                return StatusCode(StatusCodes.Status200OK, result.Data);

            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }


        [HttpPut("{id}/execute"), Authorize]
        public async Task<IActionResult> ExecuteAsync(int id)
        {
            try
            {
                var loggedUser = await FindLoggedUserAsync();
                var result = await this.transferService.ExecuteAsync(id, loggedUser);

                if (!result.IsSuccessful)
                {
                    return BadRequest(result.Message);
                }
                return StatusCode(StatusCodes.Status200OK, result.Data);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }
        private async Task<User> FindLoggedUserAsync()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = await this.userRepository.GetByUsernameAsync(loggedUsersUsername);
            return loggedUser;
        }
    }
}
