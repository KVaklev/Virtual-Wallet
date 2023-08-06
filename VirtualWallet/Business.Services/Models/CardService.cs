using AutoMapper;
using Business.DTOs;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Exceptions;
using Business.Mappers;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using static Business.Services.Helpers.Constants;

namespace Business.Services.Models
{
    public class CardService : ICardService
    {
        private readonly ICardRepository cardRepository;
        private readonly ICurrencyService currencyService;
        private readonly IMapper mapper;

        public CardService(
            ICardRepository repository,
            ICurrencyService currencyService,
            IMapper mapper)
        {
            this.cardRepository = repository;
            this.currencyService = currencyService;
            this.mapper = mapper;
        }
        public Response<IQueryable<GetCardDto>> GetAll()
        {
            var result = new Response<IQueryable<GetCardDto>>();
            var cards = this.cardRepository.GetAll();

            if (cards != null && cards.Any())
            {
                result.IsSuccessful = true;
                result.Data = (IQueryable<GetCardDto>)cards.AsQueryable();
            }
            else
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoCardsErrorMessage;
            }

            return result;
        }
        public async Task<Response<PaginatedList<GetCreatedCardDto>>> FilterByAsync(CardQueryParameters filterParameters)
        {
            var result = new Response<PaginatedList<GetCreatedCardDto>>();

            var cards = await this.cardRepository.FilterByAsync(filterParameters);

            if (cards.Count == 0)
            {
                result.IsSuccessful = false;
                result.Message = NoCardsErrorMessage;
                return result;
            }

            var cardTotalPages = cards.TotalPages;
            var cardPageNumber = cards.PageNumber;

            List<GetCreatedCardDto> cardDtos = cards
                    .Select(card => mapper.Map<GetCreatedCardDto>(card))
                    .ToList();

            result.Data = new PaginatedList<GetCreatedCardDto>(cardDtos, cardTotalPages, cardPageNumber);

            return result;
        }
        public async Task<Response<GetCardDto>> GetByIdAsync(int id, User loggedUser)
        {
            var result = new Response<GetCardDto>();

            var card = await this.cardRepository.GetByIdAsync(id);

            if (card == null)
            {
                result.IsSuccessful = false;
                result.Message = NoCardsErrorMessage;
            }
            
            if (!await Security.IsUserAuthorized(id, loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = ModifyCardErrorMessage;
                return result;
            }

            var cardDto = this.mapper.Map<GetCardDto>(card);
            result.Data = cardDto;

            return result;
        }

        public Response<IQueryable<GetCardDto>> GetByAccountId(int accountId)
        {
            var result = new Response<IQueryable<GetCardDto>>();

            var cards = this.cardRepository.GetByAccountId(accountId);

            if (cards != null && cards.Any())
            {
                result.IsSuccessful = true;
                result.Data = (IQueryable<GetCardDto>)cards.AsQueryable();
            }
            else
            {
                result.IsSuccessful = false;
                result.Message = NoCardsByAccountSearchErrorMessage;
            }

            return result;
        }
        public async Task<Response<GetCreatedCardDto>> CreateAsync(int accountId, CreateCardDto card)
        {
            var result = new Response<GetCreatedCardDto>();

            if (await CardNumberExistsAsync(card.CardNumber))
            {
                result.IsSuccessful = false;
                result.Message = CardNumberAddErrorMessage;
                result.Error = new Error(PropertyName.CardNumber);
                return result;
            }

            var cardToCreate = new Card();
            var currency = await currencyService.GetByCurrencyCodeAsync(card.CurrencyCode);
            cardToCreate = await CardsMapper.MapCreateDtoToCardAsync(accountId, cardToCreate, currency, card);
            var createdCard = await this.cardRepository.CreateAsync(accountId, cardToCreate);
            
            var cardDto = this.mapper.Map<GetCreatedCardDto>(createdCard);

            result.Data = cardDto;

            return result;
        }

        public async Task<Response<GetUpdatedCardDto>> UpdateAsync(int id, User loggedUser, UpdateCardDto updateCardDto)
        {
            var result = new Response<GetUpdatedCardDto>();

            Card cardToUpdate = await this.cardRepository.GetByIdAsync(id);

            if (await CardNumberExistsAsync(updateCardDto.CardNumber))
            {
                result.IsSuccessful = false;
                result.Message = CardNumberAddErrorMessage;
                result.Error = new Error(PropertyName.CardNumber);
                return result;
            }

            if (!await Security.IsAuthorizedAsync(cardToUpdate, loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = ModifyCardErrorMessage;
                return result;
            }

            var currency = await currencyService.GetCurrencyByIdAsync((int)cardToUpdate.CurrencyId);
            cardToUpdate = await CardsMapper.MapUpdateDtoToCardAsync(cardToUpdate, updateCardDto, currency);
            cardToUpdate = await this.cardRepository.UpdateAsync(id, cardToUpdate);

            var cardDto = mapper.Map<GetUpdatedCardDto>(cardToUpdate);
            result.Data = cardDto;

            return result;
        }
        public async Task<Response<bool>> DeleteAsync(int id, User loggedUser)
        {
            var result = new Response<bool>();

            if (!await Security.IsAdminAsync(loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = ModifyUserErrorMessage;
                return result;
            }

            result.Data = await this.cardRepository.DeleteAsync(id);
            result.Message = SuccessfullDeletedCardMessage;

            return result;
        }

        public async Task<Response<Card>> IncreaseBalanceAsync(int id, decimal amount, User loggedUser)
        {
            var result = new Response<Card>();

            Card cardToDepositTo = await this.cardRepository.GetByIdAsync(id);
            
            if (cardToDepositTo == null)
            {
                result.IsSuccessful = false;
                result.Message = NoCardFoundErrorMessage;   
                return result;

            }

            cardToDepositTo.Balance += amount;
            await this.cardRepository.SaveChangesAsync();

            result.Data = cardToDepositTo;
            return result;
        }

        public async Task<Response<Card>> DecreaseBalanceAsync(int id, decimal amount, User loggedUser)
        {
            var result = new Response<Card>();

            Card cardToWithdrawFrom = await this.cardRepository.GetByIdAsync(id);

            if (cardToWithdrawFrom == null)
            {
                result.IsSuccessful = false;
                result.Message = NoCardFoundErrorMessage;
                return result;

            }

            cardToWithdrawFrom.Balance -= amount;
            await this.cardRepository.SaveChangesAsync();

            result.Data = cardToWithdrawFrom;
            return result;
        }

        private async Task<bool> CardNumberExistsAsync(string cardNumber)
        {
            return await this.cardRepository.CardNumberExistsAsync(cardNumber);
        }
    }
}
