using Business.QueryParameters;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<PaginatedList<User>> FilterByAsync(UserQueryParameters queryParameters);
        Task<User> GetByIdAsync(int id);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByPhoneNumberAsync(string phoneNumber);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(int id, User user);
        Task<bool> DeleteAsync(int id);
        Task<User> PromoteAsync(int id);
        Task<User> BlockUserAsync(int id);
        Task<User> UnblockUserAsync(int id);
        Task UpdateAdminStatusAsync(User user, User userToUpdate);
        Task UpdatePhoneNumberAsync(User user, User userToUpdate);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneNumberExistsAsync(string phoneNumber);
    }
}
