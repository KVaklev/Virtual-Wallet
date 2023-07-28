using Business.QueryParameters;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface ICardRepository
    {
        Task<List<Card>> GetAllAsync();
        Task<PaginatedList<Card>> FilterByAsync(CardQueryParameters queryParameters);
        Task<Card> GetByIdAsync(int id);
        Task<List<Card>> GetByAccountIdAsync(int accountId);
        Task<Card> CreateAsync(int accountId, Card card);
        Task<Card> UpdateAsync(int id, Card card);
        Task<bool> CardNumberExistsAsync(string cardNumber);
        Task<Card> IncreaseBalanceAsync(int id, decimal amount);
        Task<Card> DecreaseBalanceAsync(int id, decimal amount);
        Task<bool> DeleteAsync(int id);

    }
}
