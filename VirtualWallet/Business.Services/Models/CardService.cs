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
        public List<Card> GetByUserId(int userId)
        {
            return this.repository.GetByUserId(userId);
        }
        public List<Card> FilterBy(CardQueryParameters filterParameters)
        {
            throw new NotImplementedException();
        }
        public Card Add(int userId, Card card)
        {
            throw new NotImplementedException();
        }

        public Card Update()
        {
            throw new NotImplementedException();
        }
        public void Delete()
        {
            throw new NotImplementedException();
        }




    }
}
