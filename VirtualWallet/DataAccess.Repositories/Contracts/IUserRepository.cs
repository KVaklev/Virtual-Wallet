using Business.QueryParameters;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface IUserRepository
    {
        IQueryable<User> GetAll();
        Task<User> GetByIdAsync(int id);
        Task<User> GetByUsernameAsync(string username);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User updatedUser);
        Task<bool> DeleteAsync(int id);
        Task<User> PromoteAsync(int id);
        Task<User> BlockUserAsync(int id);
        Task<User> UnblockUserAsync(int id);
        Task<bool> SaveChangesAsync();
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneNumberExistsAsync(string phoneNumber);
    }
}
