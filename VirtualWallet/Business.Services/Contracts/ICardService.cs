using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface ICardService
    {
        Task<List<Card>> GetAllAsync();
        Task<PaginatedList<Card>> FilterByAsync(CardQueryParameters queryParameters);
        Task<Card> GetByIdAsync(int id);
        Task<List<Card>> GetByAccountIdAsync(int accountId);
        Task<Card> CreateAsync(int accountId, Card card);
        Task<Card> UpdateAsync(int id, User loggedUser, Card card);
        Task<bool> DeleteAsync(int id, User loggedUser);
        Task<bool> CardNumberExistsAsync(string cardNumber);
       
    }
}
