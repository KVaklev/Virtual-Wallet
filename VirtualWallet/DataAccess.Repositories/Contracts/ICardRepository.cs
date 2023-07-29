using Business.QueryParameters;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface ICardRepository
    {
        IQueryable<Card> GetAll();
        Task<PaginatedList<Card>> FilterByAsync(CardQueryParameters queryParameters);
        Task<Card> GetByIdAsync(int id);
        IQueryable<Card> GetByAccountId(int accountId);
        Task<Card> CreateAsync(int accountId, Card card);
        Task<Card> UpdateAsync(int id, Card card);
        Task<bool> CardNumberExistsAsync(string cardNumber);
        Task<Card> IncreaseBalanceAsync(int id, decimal amount);
        Task<Card> DecreaseBalanceAsync(int id, decimal amount);
        Task<bool> DeleteAsync(int id);

    }
}
