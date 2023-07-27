using AutoMapper;
using Business.Dto;
using Business.Exceptions;
using Business.Mappers;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Models;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Helpers;

namespace VirtualWallet.Controllers.API
{
    [ApiController]
    [Route("api/cards")]
    public class CardsApiController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IAuthManager authManager;
        private readonly ICardService cardService;

        public CardsApiController(IMapper mapper, IAuthManager authManager, ICardService cardService)
        {
            this.mapper = mapper;
            this.authManager = authManager;
            this.cardService = cardService;

        }

        [HttpGet, Authorize]
        public IActionResult GetCards([FromQuery] CardQueryParameters cardQueryParameters)
        {
            try
            {
                List<Card> result = cardService.FilterBy(cardQueryParameters);

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
        public IActionResult GetById(int id)
        {
            try
            {
                Card card = cardService.GetById(id);
                GetCardDto cardDto = mapper.Map<GetCardDto>(card);

                return StatusCode(StatusCodes.Status200OK, cardDto);
            }
            catch (EntityNotFoundException e)
            {
                return StatusCode(StatusCodes.Status404NotFound, e.Message);
            }
        }

        [HttpPost, Authorize]
        public IActionResult CreateCard([FromBody] CreateCardDto createCardDto)
        {
            try
            {
                var loggedUsersAccountId = FindLoggedUsersAccount();
                Card createCard = mapper.Map<Card>(createCardDto);
                Card createdCard = cardService.Create(loggedUsersAccountId, createCard);

                return StatusCode(StatusCodes.Status201Created, createdCard);
            }
            catch (DuplicateEntityException e)
            {
                return StatusCode(StatusCodes.Status409Conflict, e.Message);
            }
        }

        private int FindLoggedUsersAccount()
        {
            var loggedUsersAccountIdAsString = User.Claims.FirstOrDefault(claim => claim.Type == "UsersAccountId").Value;
            var accountId = int.Parse(loggedUsersAccountIdAsString);
            return accountId;
        }

    }
}
