using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;

namespace Business.Services.Models
{
    public class CardService : ICardService
    {
        private readonly ICardRepository cardRepository;

        public CardService(ICardRepository repository)
        {
            this.cardRepository = repository;
        }
        public async Task<List<Card>> GetAllAsync()
        {
            return await this.cardRepository.GetAllAsync();
        }
        public async Task<PaginatedList<Card>> FilterByAsync(CardQueryParameters filterParameters)
        {
            return await this.cardRepository.FilterByAsync(filterParameters);
        }
        public async Task<Card> GetByIdAsync(int id)
        {
            return await this.cardRepository.GetByIdAsync(id);
        }
        public async Task<List<Card>> GetByAccountIdAsync(int accountId)
        {
            return await this.cardRepository.GetByAccountIdAsync(accountId);
        }
        public async Task<Card> CreateAsync(int accountId, Card card)
        {
            if (await CardNumberExistsAsync(card.CardNumber))
            {
                throw new DuplicateEntityException($"Card with card number '{card.CardNumber}' already exists.");
            }

           var createdCard = await this.cardRepository.CreateAsync(accountId, card);
           return createdCard;
        }

        public async Task<Card> UpdateAsync(int id, User loggedUser, Card card)
        {
            Card cardToUpdate = await this.cardRepository.GetByIdAsync(id);

            if (await CardNumberExistsAsync(card.CardNumber))
            {
                throw new DuplicateEntityException($"Card with card number '{card.CardNumber}' already exists.");
            }
            if (!await Common.IsAuthorizedAsync(cardToUpdate, loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyCardErrorMessage);
            }

            var updatedCard = await this.cardRepository.UpdateAsync(id, card);
            return updatedCard;

        }
        public async Task<bool> DeleteAsync(int id, User loggedUser)
        {
            if (!await Common.IsAdminAsync(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }
            return await this.cardRepository.DeleteAsync(id);
        }
        public async Task<bool> CardNumberExistsAsync(string cardNumber)
        {
            return await this.cardRepository.CardNumberExistsAsync(cardNumber);
        }

    }
}
