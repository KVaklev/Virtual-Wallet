using AutoMapper;
using Business.DTOs;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/cards")]
    public class CardApiController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICardService cardService;
        private readonly IUserService userService;

        public CardApiController(
            IMapper mapper,
            ICardService cardService,
            IUserService userService)
        {
            this.mapper = mapper;
            this.cardService = cardService;
            this.userService = userService;
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetCardsAsync([FromQuery] CardQueryParameters cardQueryParameters)
        {
            try
            {
                List<Card> result = await cardService.FilterByAsync(cardQueryParameters);

                List<GetCardDto> cardDtos = result
                    .Select(card => mapper.Map<GetCardDto>(card))
                    .ToList();

                return StatusCode(StatusCodes.Status200OK, cardDtos);
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
                Card card = await cardService.GetByIdAsync(id, loggedUser);
                GetCardDto cardDto = mapper.Map<GetCardDto>(card);

                return StatusCode(StatusCodes.Status200OK, cardDto);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> CreateCardAsync([FromBody] CreateCardDto createCardDto)
        {
            try
            {
                var loggedUsersAccountId = await FindLoggedUsersAccountAsync();
                Card createCard = mapper.Map<Card>(createCardDto);
                Card createdCard = await cardService.CreateAsync(loggedUsersAccountId, createCard);

                return StatusCode(StatusCodes.Status201Created, createdCard);
            }
            catch (DuplicateEntityException e)
            {
                return StatusCode(StatusCodes.Status409Conflict, e.Message);
            }
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateCard(int id, [FromBody] UpdateCardDto updateCardDto)
        {
            try
            {
                User loggedUser = await FindLoggedUserAsync();
                var loggedUsersAccountId = await FindLoggedUsersAccountAsync();
                GetUpdatedCardDto updatedCard = await this.cardService.UpdateAsync(id, loggedUser, updateCardDto);

                return StatusCode(StatusCodes.Status200OK, updatedCard);
            }
            catch (UnauthorizedOperationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
            }
            catch (DuplicateEntityException e)
            {
                return StatusCode(StatusCodes.Status409Conflict, e.Message);
            }
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteCardAsync(int id)
        {
            try
            {
                User loggedUser = await FindLoggedUserAsync();
                var isDeleted = await this.cardService.DeleteAsync(id, loggedUser);

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
        private async Task<Response<GetUserDto>> FindLoggedUserAsync()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = await this.userService.GetByUsernameAsync(loggedUsersUsername);
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
