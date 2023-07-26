using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface ICardService
    {
        List<Card> GetAll();
        List<Card> GetByAccountId(int accountId);
        Card GetById(int id);
        List<Card> FilterBy(CardQueryParameters filterParameters);

        //PaginatedList<Card> FilterBy(UserQueryParameters filterParameters);
        //Card Add(int userId, Card card);
        //Card Update();
        //void Delete();
    }
}
