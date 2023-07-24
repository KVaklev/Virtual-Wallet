using Business.QueryParameters;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface ICardRepository
    {
        List<Card> GetAll();
        List<Card> GetByUserId(int userId);
        Card GetById(int id);
        Card Add(int userId, int accountId, Card card);
        Card Update(int id, Card card);
        void Delete(int id);
        List<Card> FilterBy(CardQueryParameters filterParameters);
        //PaginatedList<Card> FilterBy(UserQueryParameters filterParameters);

    }
}
