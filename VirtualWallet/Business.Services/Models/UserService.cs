using Business.Exceptions;
using Business.Services.Contracts;
using DataAccess.Models.Contracts;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Business.Services.Helpers;

namespace Business.Services.Models
{
    public class UserService : IUserService
    {
        private readonly IUserRepository repository;

        public UserService(IUserRepository repository)
        {
            this.repository = repository;
        }

        public List<IUser> GetAll()
        {
            return this.repository.GetAll();
        }
        public IUser GetById(int id)
        {
            return this.repository.GetById(id);
        }
        public IUser GetByUsername(string username)
        {
            return this.repository.GetByUsername(username);
        }
        public IUser GetByEmail(string email)
        {
            return this.repository.GetByEmail(email);
        }
        public IUser GetByPhoneNumber(int pnoneNumber)
        {
            return this.repository.GetByPhoneNumber(pnoneNumber);
        }

        //public PaginatedList<User> FilterBy(UserQueryParameters filterParameters)
        //{
        //    return this.repository.FilterBy(filterParameters);
        //}
        public IUser Create(IUser user)
        {
            if (UsernameExists(user.Username))
            {
                throw new DuplicateEntityException($"User with username '{user.Username}' already exists.");
            }

            if (EmailExists(user.Email))
            {
                throw new DuplicateEntityException($"User with email '{user.Email}' already exists.");
            }

            if (PhoneNumberExists(user.PhoneNumber))
            {
                throw new DuplicateEntityException($"User with phone number '{user.PhoneNumber}' already exists.");
            }

            IUser createdUser = this.repository.Create(user);

            return createdUser;

        }
        public IUser Update(int id, IUser user, IUser loggedUser)
        {
            IUser userToUpdate = this.repository.GetById(id);

            if (!IsAuthorized(userToUpdate, loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }

            if (user.Email != userToUpdate.Email)
            {
                if (EmailExists(user.Email))
                {
                    throw new DuplicateEntityException($"User with email '{user.Email}' already exists.");
                }
            }

            if (user.PhoneNumber != userToUpdate.PhoneNumber)
            {
                if (PhoneNumberExists(user.PhoneNumber))
                {
                    throw new DuplicateEntityException($"User with phone number '{user.PhoneNumber}' already exists.");
                }
            }

            userToUpdate = this.repository.Update(id, user, loggedUser);
            return userToUpdate;
        }
        public void Delete(int id, IUser loggedUser)
        {
            IUser user = repository.GetById(id);

            if (!IsAuthorized(user, loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }
            this.repository.Delete(id);
        }
        public IUser Promote(IUser user)
        {
            return this.repository.Promote(user);
        }
        public IUser BlockUser(IUser user)
        {
            return this.repository.BlockUser(user);
        }
        public IUser UnblockUser(IUser user)
        {
            return this.repository.UnblockUser(user);
        }
        public bool EmailExists(string email)
        {
            return this.repository.EmailExists(email);
        }
        public bool UsernameExists(string username)
        {
            return this.repository.UsernameExists(username);
        }
        public bool PhoneNumberExists(int phoneNumber)
        {
            return this.repository.PhoneNumberExists(phoneNumber);
        }
        public bool IsAuthorized(IUser user, IUser loggedUser)
        {
            bool isAuthorized = false;

            if (user.Id == loggedUser.Id || loggedUser.IsAdmin)
            {
                isAuthorized = true;
            }
            return isAuthorized;
        }        
    }
}
