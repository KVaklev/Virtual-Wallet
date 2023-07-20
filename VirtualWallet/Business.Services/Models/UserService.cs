using Business.Services.Contracts;
using DataAccess.Models.Contracts;

namespace Business.Services.Models
{
    public class UserService : IUserService
    {
        //private readonly IUserRepository repository;

        //public UserService(IUserRepository repository)
        //{
        //    this.repository = repository;
        //}
        public List<IUser> GetAll()
        {
            throw new NotImplementedException();
        }
        public IUser BlockUser(IUser user)
        {
            throw new NotImplementedException();
        }

        public IUser Create(IUser user)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id, IUser loggedUser)
        {
            throw new NotImplementedException();
        }

        public bool EmailExists(string email)
        {
            throw new NotImplementedException();
        }


        public IUser GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IUser GetByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorized(IUser user, IUser loggedUser)
        {
            throw new NotImplementedException();
        }

        public IUser Promote(IUser user)
        {
            throw new NotImplementedException();
        }

        public IUser UnblockUser(IUser user)
        {
            throw new NotImplementedException();
        }

        public IUser Update(int id, IUser user, IUser loggedUser)
        {
            throw new NotImplementedException();
        }

        public bool UsernameExists(string username)
        {
            throw new NotImplementedException();
        }
    }
}
