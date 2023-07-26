using Business.QueryParameters;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface ICardRepository
    {
        List<Card> GetAll();
        List<Card> GetByAccountId(int accountId);
        Card GetById(int id);
        List<Card> FilterBy(CardQueryParameters filterParameters);
        //Card Add(int userId, int accountId, Card card);
        //Card Update(int id, Card card);
        //void Delete(int id);
        //PaginatedList<Card> FilterBy(UserQueryParameters filterParameters);

    }
}
