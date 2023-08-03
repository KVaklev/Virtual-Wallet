using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface IAccountService
    {
        IQueryable<Account> GetAll();
        Task <Account> GetByIdAsync(int id, User user);
        Task <Account> GetByUsernameAsync(int id, User user);
        Task <Account> CreateAsync(string currencyCode, User user);
        Task <bool> AddCardAsync(int id, Card card, User user);
        Task <bool> RemoveCardAsync(int id,  Card card, User user);
        Task <bool> DeleteAsync(int id, User loggedUser);
        Task<string> CreateApiTokenAsync(User loggedUser);
        Task<string> GenerateTokenAsync(int id);
        Task<bool> ConfirmRegistrationAsync(int id, string token);
        Task<User> LoginAsync(string username, string password);
    }
}
