using Business.QueryParameters;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;

namespace Business.Services.Models
{
    public class CardService : ICardService
    {
        private readonly ICardRepository repository;

        public CardService(ICardRepository repository)
        {
            this.repository = repository;
        }
        public List<Card> GetAll()
        {
            return this.repository.GetAll();
        }
        public List<Card> GetByAccountId(int accountId)
        {
            return this.repository.GetByAccountId(accountId);
        }
        public Card GetById(int id)
        {
            return this.repository.GetById(id);
        }
        public List<Card> FilterBy(CardQueryParameters filterParameters)
        {
            return this.repository.FilterBy(filterParameters);
        }
        //public Card Add(int userId, Card card)
        //{
        //    throw new NotImplementedException();
        //}

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
