using Business.QueryParameters;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface IUserRepository
    {
        List<User> GetAll();
        List<User> FilterBy(UserQueryParameters filterParameters);
        // PaginatedList<IUser> FilterBy(UserQueryParameters filterParameters);
        User GetById(int id);
        User GetByUsername(string username);
        User GetByEmail(string email);
        User GetByPhoneNumber(string phoneNumber);
        User Create(User user);
        User Update(int id, User user, User loggedUser);
        User Delete(int id);
        User Promote(User user);
        User BlockUser(User user);
        User UnblockUser(User user);
        void UpdateAdminStatus(User user, User userToUpdate);
        void UpdatePhoneNumber(User user, User userToUpdate);
        bool UsernameExists(string username);
        bool EmailExists(string email);
        bool PhoneNumberExists(string phoneNumber);
    }
}
