using Business.QueryParameters;
using Business.Services.Contracts;
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
           var createdCard = this.repository.CreateCard(card);
           createdCard = this.repository.AddCardToAccount(accountId, createdCard);
           return createdCard;
        }

        //public Card Update()
        //{
        //    throw new NotImplementedException();
        //}
        //public void Delete()
        //{
        //    throw new NotImplementedException();
        //}


    }
}
