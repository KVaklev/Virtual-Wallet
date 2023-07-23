using Business.QueryParameters;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;

namespace DataAccess.Repositories.Models
{
    public class CardRepository : ICardRepository
    {
        public Card Add(int userId, Card card)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public List<Card> FilterBy(CardQueryParameters filterParameters)
        {
            throw new NotImplementedException();
        }

        public List<Card> GetAll()
        {
            throw new NotImplementedException();
        }

        public List<Card> GetByUserId(int userId)
        {
            throw new NotImplementedException();
        }

        public Card Update()
        {
            throw new NotImplementedException();
        }
    }
}
