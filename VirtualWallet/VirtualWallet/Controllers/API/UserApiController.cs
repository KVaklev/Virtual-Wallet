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
    [Route("api/users")]
    public class UserApiController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;

        public UserApiController(
            IUserService userService,
            IMapper mapper,
            IUserRepository userRepository)
        {
            this.userService = userService;
            this.mapper = mapper;
            this.userRepository = userRepository;
        }
       
        [HttpGet, Authorize]
        public async Task<IActionResult> GetUsersAsync([FromQuery] UserQueryParameters userQueryParameters)
        {
            try
            {
                var result = await userService.FilterByAsync(userQueryParameters);

                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpGet("id"),Authorize]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                User loggedUser = await FindLoggedUserAsync();
                var result = await this.userService.GetByIdAsync(id, loggedUser);

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

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserDto createUserDto)
        {
            var result = await userService.CreateAsync(createUserDto);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }
          
            return StatusCode(StatusCodes.Status201Created, result.Data);
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            User loggedUser = await FindLoggedUserAsync();
            var result = await userService.UpdateAsync(id, updateUserDto, loggedUser);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status200OK, result.Data);
           
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                User loggedUser = await FindLoggedUserAsync();
                var result = await userService.DeleteAsync(id, loggedUser);

                if (!result.IsSuccessful)
                {
                    return BadRequest(result.Message);
                }

                result.Message = Constants.SuccessfullDeletedUserMessage;

                return StatusCode(StatusCodes.Status200OK, result.Message);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpPut("{id}/promote"), Authorize]
        public async Task<IActionResult> Promote(int id)
        {
            try
            {
                User loggedUser = await FindLoggedUserAsync();
                var result = await userService.PromoteAsync(id, loggedUser);

                if (!result.IsSuccessful)
                {
                    return BadRequest(result.Message);
                }

                result.Message = Constants.SuccessfullPromoteddUserMessage;

                return StatusCode(StatusCodes.Status200OK, result.Message);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpPut("{id}/block"), Authorize]
        public async Task<IActionResult> BlockUser(int id)
        {
            try
            {
                User loggedUser = await FindLoggedUserAsync();
                var result = await userService.BlockUserAsync(id, loggedUser);

                if (!result.IsSuccessful)
                {
                    return BadRequest(result.Message);
                }

                result.Message = Constants.SuccessfullBlockedUserMessage;

                return StatusCode(StatusCodes.Status200OK, result.Message);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpPut("{id}/unblock"), Authorize]
        public async Task<IActionResult> UnblockUser(int id)
        {
            try
            {
                User loggedUser = await FindLoggedUserAsync();
                var result = await userService.UnblockUserAsync(id, loggedUser);

                if (!result.IsSuccessful)
                {
                    return BadRequest(result.Message);
                }

                result.Message = Constants.SuccessfullUnblockedUserMessage;

                return StatusCode(StatusCodes.Status200OK, result.Message);
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
