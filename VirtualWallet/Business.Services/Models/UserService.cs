using Business.Exceptions;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Business.Services.Helpers;
using Business.QueryParameters;

namespace Business.Services.Models
{
    public class UserService : IUserService
    {
        private readonly IUserRepository repository;

        public UserService(IUserRepository repository)
        {
            this.repository = repository;
        }

        //public List<User> GetAll()
        //{
        //    return this.repository.GetAll();
        //}

        public async Task<List<User>> GetAllAsync()
        {
            return await this.repository.GetAllAsync();
        }
        //public List<User> FilterBy(UserQueryParameters filterParameters)
        //{
        //    return this.repository.FilterBy(filterParameters);
        //}

        public async Task<List<User>> FilterByAsync(UserQueryParameters filterParameters)
        {
            return await this.repository.FilterByAsync(filterParameters);
        }

        public User GetById(int id)
        {
            return this.repository.GetById(id);
        }
        public User GetByUsername(string username)
        {
            return this.repository.GetByUsername(username);
        }
        public User GetByEmail(string email)
        {
            return this.repository.GetByEmail(email);
        }
        public User GetByPhoneNumber(string pnoneNumber)
        {
            return this.repository.GetByPhoneNumber(pnoneNumber);
        }

        //public PaginatedList<User> FilterBy(UserQueryParameters filterParameters)
        //{
        //    return this.repository.FilterBy(filterParameters);
        //}
        public User Create(User user)
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

            User createdUser = this.repository.Create(user);

            return createdUser;

        }
        public User Update(int id, User user, User loggedUser)
        {
            User userToUpdate = this.repository.GetById(id);

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

        //todo
        public void Delete(int id, User loggedUser)
        {
            EnsureAdminAuthorization(loggedUser);
            this.repository.Delete(id);
        }
        public User Promote(int id, User loggedUser)
        {
            EnsureAdminAuthorization(loggedUser);
            return this.repository.Promote(id);
        }
        public User BlockUser(int id, User loggedUser)
        {
            EnsureAdminAuthorization(loggedUser);
            return this.repository.BlockUser(id);
        }
        public User UnblockUser(int id, User loggedUser)
        {
            EnsureAdminAuthorization(loggedUser);
            return this.repository.UnblockUser(id);
        }
        public bool EmailExists(string email)
        {
            return this.repository.EmailExists(email);
        }
        public bool UsernameExists(string username)
        {
            return this.repository.UsernameExists(username);
        }
        public bool PhoneNumberExists(string phoneNumber)
        {
            return this.repository.PhoneNumberExists(phoneNumber);
        }
        public bool IsAuthorized(User user, User loggedUser)
        {
            bool isAuthorized = false;

            if (user.Id == loggedUser.Id || loggedUser.IsAdmin)
            {
                isAuthorized = true;
            }
            return isAuthorized;
        }
        public void EnsureAdminAuthorization(User loggedUser)
        {
            if (!loggedUser.IsAdmin)
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }
        }
    }
}
