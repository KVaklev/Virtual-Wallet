using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface ICardRepository
    {
        IQueryable<Card> GetAll();
        
        Task<Card> GetByIdAsync(int id);
        
        IQueryable<Card> GetByAccountId(int accountId);
        
        Task<Card> CreateAsync(int accountId, Card card);
        
        Task<Card> UpdateAsync(int id, Card card);
        
        Task<bool> CardNumberExistsAsync(string cardNumber);
        
        Task<bool> SaveChangesAsync();
        
        Task<bool> DeleteAsync(int id);
    }
}
