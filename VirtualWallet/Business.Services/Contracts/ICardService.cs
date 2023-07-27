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
        Card Create(int accountId, Card card);
        bool CardNumberExists(string cardNumber);

        //PaginatedList<Card> FilterBy(UserQueryParameters filterParameters);
        //Card Update();
        //void Delete();
    }
}
