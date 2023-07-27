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
        private readonly ICardRepository repository;
        private readonly IAccountRepository accountRepository;

        public CardService(ICardRepository repository, IAccountRepository accountRepository)
        {
            this.repository = repository;
            this.accountRepository = accountRepository;
        }
        public List<Card> GetAll()
        {
            return this.repository.GetAll();
        }
        public Card GetById(int id)
        {
            return this.repository.GetById(id);
        }
        public List<Card> GetByAccountId(int accountId)
        {
            return this.repository.GetByAccountId(accountId);
        }
        public List<Card> FilterBy(CardQueryParameters filterParameters)
        {
            return this.repository.FilterBy(filterParameters);
        }
        public Card Create(int accountId, Card card)
        {
            if (CardNumberExists(card.CardNumber))
            {
                throw new DuplicateEntityException($"Card with card number '{card.CardNumber}' already exists.");
            }

           var createdCard = this.repository.Create(accountId, card);
           return createdCard;
        }

        public Card Update(int id, User loggedUser, Card card)
        {
            Card cardToUpdate = this.repository.GetById(id);

            if (CardNumberExists(card.CardNumber))
            {
                throw new DuplicateEntityException($"Card with card number '{card.CardNumber}' already exists.");
            }
            if (!IsAuthorized(cardToUpdate, loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyCardErrorMessage);
            }

            var updatedCard = this.repository.Update(id, card);
            return updatedCard;

        }
        public bool CardNumberExists(string cardNumber)
        {
            return this.repository.CardNumberExists(cardNumber);
        }

        public bool IsAuthorized(Card card, User loggedUser)
        {
            bool isAuthorized = false;

            if (card.Account.User.Id == loggedUser.Id || loggedUser.IsAdmin)
            {
                isAuthorized = true;
            }
            return isAuthorized;
        }
    }
}
