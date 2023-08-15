using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Mappers;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
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
        private Response<IQueryable<Card>> GetAll(User loggedUser)
        {
            var result = new Response<IQueryable<Card>>();
            var cards = this.cardRepository.GetAll();

            if (cards.Any())
            {
                if (!loggedUser.IsAdmin)
                {
                    cards = cards
                        .Where(a => a.Account.User.Username == loggedUser.Username)
                        .AsQueryable();
                }
                result.Data = cards;
                
                return result;
            }
            else
            {
                result.IsSuccessful = false;
                result.Message = NoCardsErrorMessage;
                return result;
            }
        }
        public async Task<Response<PaginatedList<GetCreatedCardDto>>> FilterByAsync(CardQueryParameters filterParameters, User loggedUser)
        {
            var result = new Response<PaginatedList<GetCreatedCardDto>>();
            var cardsResult = this.GetAll(loggedUser);
            IQueryable<Card> cards = cardsResult.Data;

            cards = await FilterByUsernameAsync(cards, filterParameters.Username);
            cards = await FilterByExpirationDateAsync(cards, filterParameters.ExpirationDate);
            cards = await FilterByCardTypeAsync(cards, filterParameters.CardType);
            cards = await FilterByBalanceAsync(cards, filterParameters.Balance);
            cards = await SortByAsync(cards, filterParameters.SortBy);
            cards = await SortOrderAsync(cards, filterParameters.SortOrder);

            int totalPages = (cards.Count() + filterParameters.PageSize - 1) / filterParameters.PageSize;
            cards = await Common<Card>.PaginateAsync(cards, filterParameters.PageNumber, filterParameters.PageSize);

            if (!cards.Any())
            {
                result.IsSuccessful = false;
                result.Message = NoCardsErrorMessage;
                return result;
            }

            List<GetCreatedCardDto> cardDtos = cards
               .Select(card => mapper.Map<GetCreatedCardDto>(card))
               .ToList();

            result.Data = new PaginatedList<GetCreatedCardDto>(cardDtos, totalPages, filterParameters.PageNumber);
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
            
            if (!await Security.IsUserAuthorized(card.Account.Id, loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = ModifyCardErrorMessage;
                return result;
            }

            var cardDto = this.mapper.Map<GetCardDto>(card);
            result.Data = cardDto;

            return result;
        }

        public Response<List<GetCardDto>> GetByAccountId(int accountId)
        {
            var result = new Response<List<GetCardDto>>();

            var cards = this.cardRepository.GetByAccountId(accountId);

            var cardDto = cards
                .Select(cards => this.mapper.Map<GetCardDto> (cards))
                .ToList();

            if (cards != null && cards.Any())
            {
                result.IsSuccessful = true;
                result.Data = cardDto;
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
            cardToCreate = await CardsMapper.MapCreateDtoToCardAsync(accountId, cardToCreate, currency.Data, card);
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
            cardToUpdate = await CardsMapper.MapUpdateDtoToCardAsync(cardToUpdate, updateCardDto, currency.Data);
            cardToUpdate = await this.cardRepository.UpdateAsync(id, cardToUpdate);

            var cardDto = mapper.Map<GetUpdatedCardDto>(cardToUpdate);
            result.Data = cardDto;

            return result;
        }
        public async Task<Response<bool>> DeleteAsync(int id, User loggedUser)
        {
            var result = new Response<bool>();

            var cardResult = await this.GetByIdAsync(id, loggedUser);
            if (!cardResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = NoCardFoundErrorMessage;
                return result;
            }

            var accountId = cardResult.Data.AccountId;
            if (!await Security.IsUserAuthorized(accountId,loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = ModifyUserErrorMessage;
                return result;
            }

            result.Data = await this.cardRepository.DeleteAsync(id);
            if (!result.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = ModifyUserErrorMessage;
                return result;
            }
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

        private async Task<IQueryable<Card>> FilterByUsernameAsync(IQueryable<Card> result, string username)
        {
            result = result
                .Include(c => c.Account)
                .ThenInclude(a => a.User);

            if (!string.IsNullOrEmpty(username))
            {
                return await Task.FromResult(result.Where(card => card.Account.User.Username.Contains(username.ToUpper())));
            }

            return await Task.FromResult(result);
        }
        private async Task<IQueryable<Card>> FilterByExpirationDateAsync(IQueryable<Card> result, string expirationDate)
        {
            DateTime? date = !string.IsNullOrEmpty(expirationDate) ? DateTime.Parse(expirationDate) : null;
            return await Task.FromResult(result.Where(t => !date.HasValue || t.ExpirationDate <= date));
        }
        private async Task<IQueryable<Card>> FilterByCardTypeAsync(IQueryable<Card> result, string cardTypeString)
        {
            if (!string.IsNullOrEmpty(cardTypeString) && Enum.TryParse(cardTypeString, true, out CardType cardType))
            {
                result = result.Where(card => card.CardType == cardType);
            }

            return await Task.FromResult(result);
        }
        private async Task<IQueryable<Card>> FilterByBalanceAsync(IQueryable<Card> result, decimal? balance)
        {
            if (balance.HasValue)
            {
                result = result.Where(card => card.Balance <= balance);
            }

            return await Task.FromResult(result);
        }
        private async Task<IQueryable<Card>> SortByAsync(IQueryable<Card> result, string sortCriteria)
        {
            if (Enum.TryParse<SortCriteria>(sortCriteria, true, out var sortEnum))
            {
                switch (sortEnum)
            {
                case SortCriteria.Username:
                    return await Task.FromResult(result.OrderBy(card => card.Account.User.Username));
                case SortCriteria.ExpirationDate:
                    return await Task.FromResult(result.OrderBy(card => card.ExpirationDate));
                case SortCriteria.CardType:
                    return await Task.FromResult(result.OrderBy(card => card.CardType));
                case SortCriteria.Balance:
                    return await Task.FromResult(result.OrderBy(card => card.Account.Balance));
                }
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<Card>> SortOrderAsync(IQueryable<Card> result, string sortOrder)
        {
            if (Enum.TryParse<SortCriteria>(sortOrder, true, out var sortEnum))
            {
                switch (sortEnum)
                {
                    case SortCriteria.Desc:
                        return await Task.FromResult(result.Reverse());
                }
            }
            return await Task.FromResult(result);
        }
    }
}
