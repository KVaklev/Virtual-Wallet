using DataAccess.Models.Contracts;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface IUserRepository
    {
        List<IUser> GetAll();
        // PaginatedList<IUser> FilterBy(UserQueryParameters filterParameters);
        IUser GetById(int id);
        IUser GetByUsername(string username);
        IUser GetByEmail(string email);
        IUser GetByPhoneNumber(int phoneNumber);
        IUser Create(IUser user);
        IUser Update(int id, IUser user, IUser loggedUser);
        IUser Delete(int id);
        IUser Promote(IUser user);
        IUser BlockUser(IUser user);
        IUser UnblockUser(IUser user);
        void UpdateAdminStatus(IUser user, IUser userToUpdate);
        void UpdatePhoneNumber(IUser user, IUser userToUpdate);
        bool UsernameExists(string username);
        bool EmailExists(string email);
        bool PhoneNumberExists(int phoneNumber);
    }
}
