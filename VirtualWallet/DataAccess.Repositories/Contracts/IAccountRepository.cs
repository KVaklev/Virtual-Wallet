using DataAccess.Models.Models;


namespace DataAccess.Repositories.Contracts
{
    public interface IAccountRepository
    {
        IQueryable<Account> GetAll();
        
        Task<Account> GetByIdAsync(int id);
        
        Task<Account> GetByUsernameAsync(string username);
        
        Task<Account> CreateAsync(Account account);
        
        Task<bool> AddCardAsync(int id, Card card);
        
        Task<bool> RemoveCardAsync(int id, Card card);
        
        Task<bool> DeleteAsync(int id);
        
        Task<bool> SaveChangesAsync();
        
        Task<bool> ConfirmRegistrationAsync(int id, string token);
    }
}
