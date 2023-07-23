using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface ICardService
    {
        List<Card> GetAll();

        List<Card> FilterBy(CardQueryParameters filterParameters);
        //PaginatedList<Card> FilterBy(UserQueryParameters filterParameters);
        List<Card> GetByUserId(int userId);

        Card Add(int userId, Card card);
        Card Update();
        void Delete();
    }
}
