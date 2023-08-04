using AutoMapper;
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
    [Route("api/cards")]
    public class CardApiController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICardService cardService;
        private readonly IUserRepository userRepository;

        public CardApiController(
            IMapper mapper,
            ICardService cardService,
            IUserRepository userRepository)
        {
            this.mapper = mapper;
            this.cardService = cardService;
            this.userRepository = userRepository;
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetCardsAsync([FromQuery] CardQueryParameters cardQueryParameters)
        {
            try
            {
                List<Card> result = await cardService.FilterByAsync(cardQueryParameters);

                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpGet("id"), Authorize]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                User loggedUser = await FindLoggedUserAsync();
                var result = await cardService.GetByIdAsync(id, loggedUser);

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

        [HttpPost, Authorize]
        public async Task<IActionResult> CreateCardAsync([FromBody] CreateCardDto createCardDto)
        {
        
            var loggedUsersAccountId = await FindLoggedUsersAccountAsync();

            var result = await cardService.CreateAsync(loggedUsersAccountId, createCardDto);

            if (!result.IsSuccessful) 
            {
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status201Created, result);          
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateCard(int id, [FromBody] UpdateCardDto updateCardDto)
        {
          
            User loggedUser = await FindLoggedUserAsync();
            var result = await this.cardService.UpdateAsync(id, loggedUser, updateCardDto);

            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            return StatusCode(StatusCodes.Status200OK, result);
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteCardAsync(int id)
        {
            try
            {
                User loggedUser = await FindLoggedUserAsync();
                var result = await this.cardService.DeleteAsync(id, loggedUser);

                if (!result.IsSuccessful)
                {
                    return BadRequest(result.Message);
                }

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
        private async Task<int> FindLoggedUsersAccountAsync()
        {
            var loggedUsersAccountIdAsString = User.Claims.FirstOrDefault(claim => claim.Type == "UsersAccountId").Value;
            var accountId = int.Parse(loggedUsersAccountIdAsString);
            return accountId;
        }

    }
}
