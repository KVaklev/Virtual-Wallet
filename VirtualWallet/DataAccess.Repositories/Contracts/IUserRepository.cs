using Business.QueryParameters;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface IUserRepository
    {
        //List<User> GetAll();
        Task<List<User>> GetAllAsync();
        //List<User> FilterBy(UserQueryParameters filterParameters);
        Task<List<User>>FilterByAsync(UserQueryParameters queryParameters);
        // PaginatedList<IUser> FilterBy(UserQueryParameters filterParameters);
        User GetById(int id);
        User GetByUsername(string username);
        User GetByEmail(string email);
        User GetByPhoneNumber(string phoneNumber);
        User Create(User user);
        User Update(int id, User user, User loggedUser);
        User Delete(int id);
        User Promote(int id);
        User BlockUser(int id);
        User UnblockUser(int id);
        void UpdateAdminStatus(User user, User userToUpdate);
        void UpdatePhoneNumber(User user, User userToUpdate);
        bool UsernameExists(string username);
        bool EmailExists(string email);
        bool PhoneNumberExists(string phoneNumber);
    }
}
