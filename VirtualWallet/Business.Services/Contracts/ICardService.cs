using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface ICardService
    {
        List<Card> GetAll();
        Card GetById(int id);
        List<Card> GetByAccountId(int accountId);
        List<Card> FilterBy(CardQueryParameters filterParameters);
      //  Card Add(int accountId, Card card);

        //PaginatedList<Card> FilterBy(UserQueryParameters filterParameters);
        //Card Update();
        //void Delete();
    }
}
