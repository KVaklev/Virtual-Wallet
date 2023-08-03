using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Exceptions;
using Business.Mappers;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Models;

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
        public IQueryable<Card> GetAll()
        {
            return this.cardRepository.GetAll();
        }
        public async Task<PaginatedList<Card>> FilterByAsync(CardQueryParameters filterParameters)
        {
            return await this.cardRepository.FilterByAsync(filterParameters);
        }
        public async Task<Card> GetByIdAsync(int id, User loggedUser)
        {
            if (!await Security.IsUserAuthorized(id, loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyCardErrorMessage);
            }
            return await this.cardRepository.GetByIdAsync(id);
        }
        public IQueryable<Card> GetByAccountId(int accountId)
        {
            return this.cardRepository.GetByAccountId(accountId);
        }
        public async Task<Card> CreateAsync(int accountId, Card card)
        {
            if (await CardNumberExistsAsync(card.CardNumber))
            {
                throw new DuplicateEntityException($"Card with card number '{card.CardNumber}' already exists.");
            }
            var cardToCreate = new Card();
            var currency = await currencyService.GetByCurrencyCodeAsync(card.Currency.CurrencyCode);
            cardToCreate = await CardsMapper.MapCreateDtoToCardAsync(accountId, cardToCreate, currency, card);
            var createdCard = await this.cardRepository.CreateAsync(accountId, cardToCreate);
            
            return createdCard;
        }

        public async Task<GetUpdatedCardDto> UpdateAsync(int id, User loggedUser, UpdateCardDto updateCardDto)
        {
            Card cardToUpdate = await this.cardRepository.GetByIdAsync(id);

            if (await CardNumberExistsAsync(updateCardDto.CardNumber))
            {
                throw new DuplicateEntityException($"Card with card number '{updateCardDto.CardNumber}' already exists.");
            }
            if (!await Security.IsAuthorizedAsync(cardToUpdate, loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyCardErrorMessage);
            }

            var currency = await currencyService.GetCurrencyByIdAsync((int)cardToUpdate.CurrencyId);
            cardToUpdate = await CardsMapper.MapUpdateDtoToCardAsync(cardToUpdate, updateCardDto, currency);
            cardToUpdate = await this.cardRepository.UpdateAsync(id, cardToUpdate);

            GetUpdatedCardDto updatedCard = mapper.Map<GetUpdatedCardDto>(cardToUpdate);
            return updatedCard;

        }
        public async Task<bool> DeleteAsync(int id, User loggedUser)
        {
            if (!await Security.IsAdminAsync(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }
            var cardToDelete = await this.cardRepository.DeleteAsync(id);

            return cardToDelete;
        }
        public async Task<bool> CardNumberExistsAsync(string cardNumber)
        {
            return await this.cardRepository.CardNumberExistsAsync(cardNumber);
        }

        public async Task<Card> IncreaseBalanceAsync(int id, decimal amount, User loggedUser)
        {
            Card cardToDepositTo = await this.GetByIdAsync(id, loggedUser);
            cardToDepositTo.Balance += amount;

            return await this.cardRepository.IncreaseBalanceAsync(id, amount);
        }

        public async Task<Card> DecreaseBalanceAsync(int id, decimal amount, User loggedUser)
        {
            Card cardToWithdrawFrom = await this.GetByIdAsync(id, loggedUser);
            cardToWithdrawFrom.Balance -= amount;

            return await this.cardRepository.DecreaseBalanceAsync(id, amount);
        }
    }
}
