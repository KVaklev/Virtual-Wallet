using DataAccess.Models.Contracts;

namespace Business.Services.Contracts
{
    public interface IUserService
    {
        List<IUser> GetAll();
       // PaginatedList<IUser> FilterBy(UserQueryParameters filterParameters);
        IUser GetById(int id);
        IUser GetByUsername(string username);
        IUser Promote(IUser user);
        IUser BlockUser(IUser user);
        IUser UnblockUser(IUser user);
        IUser Create(IUser user);
        IUser Update(int id, IUser user, IUser loggedUser);
        void Delete(int id, IUser loggedUser);
        bool IsAuthorized(IUser user, IUser loggedUser);
        bool UsernameExists(string username);
        bool EmailExists(string email);
    }
}
