using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface IUserService
    {
        List<User> GetAll();

        List<User> FilterBy(UserQueryParameters filterParameters);
        //PaginatedList<IUser> FilterBy(UserQueryParameters filterParameters);
        User GetById(int id);
        User GetByUsername(string username);
        User GetByEmail(string email);
        User GetByPhoneNumber(int phoneNumber);
        User Create(User user);
        User Update(int id, User user, User loggedUser);
        void Delete(int id, User loggedUser);
        User Promote(User user);
        User BlockUser(User user);
        User UnblockUser(User user); 
        bool UsernameExists(string username);
        bool EmailExists(string email);
        bool PhoneNumberExists(int phoneNumber);
        bool IsAuthorized(User user, User loggedUser);
    }
}
