using AutoMapper;
using Business.Dto;
using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/users")]
    public class UsersApiController : Controller
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;

        public UsersApiController(IUserService userService, IMapper mapper, IAuthManager authManager)
        {
            this.userService = userService;
            this.mapper = mapper;
            this.authManager = authManager;
        }
        [HttpGet("")]
        public IActionResult GetUsers([FromHeader] string credentials, [FromQuery] UserQueryParameters userQueryParameters)
        {
            User loggedUser = authManager.TryGetUser(credentials);

            if (loggedUser == null || !loggedUser.IsAdmin)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            //List<User> result = userService.FilterBy(userQueryParameters);

            // List<GetUserDto> userDtos = result.Select(user => mapper.Map<GetUserDto>(user)).ToList();

            return StatusCode(StatusCodes.Status200OK);//, userDtos);
        }

        [HttpGet("id")]
        public IActionResult GetUserById([FromHeader] string credentials, int id)
        {
            try
            {
                User loggedUser = authManager.TryGetUser(credentials);

                if (loggedUser == null || !loggedUser.IsAdmin)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
                User user = userService.GetById(id);

                GetUserDto userDto = mapper.Map<GetUserDto>(user);

                return StatusCode(StatusCodes.Status200OK, userDto);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
            catch (EntityNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex.Message);
            }
        }

        [HttpPost("")]
        public IActionResult CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                User user = mapper.Map<User>(createUserDto);

                User createdUser = userService.Create(user);

                return StatusCode(StatusCodes.Status201Created, createdUser);
            }
            catch (DuplicateEntityException e)
            {
                return StatusCode(StatusCodes.Status409Conflict, e.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromHeader] string credentials, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                User loggedUser = authManager.TryGetUser(credentials);

                User user = mapper.Map<User>(updateUserDto);

                User updatedUser = userService.Update(id, user, loggedUser);

                return StatusCode(StatusCodes.Status200OK, updatedUser);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
            catch (DuplicateEntityException e)
            {
                return StatusCode(StatusCodes.Status409Conflict, e.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id, [FromHeader] string credentials)
        {
            try
            {
                User user = authManager.TryGetUser(credentials);

                userService.Delete(id, user);

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpPut("{id}/promote")]
        public IActionResult Promote(int id, [FromHeader] string credentials)
        {
            try
            {
                User loggedUser = authManager.TryGetUser(credentials);

                if (loggedUser.IsAdmin)
                {
                    User user = userService.GetById(id);

                    User promotedUser = userService.Promote(user);

                    return StatusCode(StatusCodes.Status200OK, promotedUser);
                }
                return StatusCode(StatusCodes.Status405MethodNotAllowed);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpPut("{id}/block")]
        public IActionResult BlockUser(int id, [FromHeader] string credentials)
        {
            try
            {
                User loggedUser = authManager.TryGetUser(credentials);

                if (loggedUser.IsAdmin)
                {
                    var user = userService.GetById(id);

                    var promotedUser = userService.BlockUser(user);

                    return StatusCode(StatusCodes.Status200OK, promotedUser);
                }
                return StatusCode(StatusCodes.Status405MethodNotAllowed);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpPut("{id}/unblock")]
        public IActionResult UnblockUser(int id, [FromHeader] string credentials)
        {
            try
            {
                var loggedUser = authManager.TryGetUser(credentials);

                if (loggedUser.IsAdmin)
                {
                    User user = userService.GetById(id);

                    User promotedUser = userService.UnblockUser(user);

                    return StatusCode(StatusCodes.Status200OK, promotedUser);
                }

                return StatusCode(StatusCodes.Status405MethodNotAllowed);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }
    }
}
