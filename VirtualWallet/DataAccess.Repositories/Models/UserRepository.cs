using Business.Exceptions;
using Business.QueryParameters;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;

namespace DataAccess.Repositories.Models
{
    public class UserRepository : IUserRepository
    {

        private readonly ApplicationContext context;

        public UserRepository(ApplicationContext context)
        {
            this.context = context;
        }
        public List<User> GetAll()
        {
            return context.Users.ToList();
        }
        public List<User> FilterBy(UserQueryParameters filterParameters)
        {
            List<User> result = this.context.Users.ToList();

            if (!string.IsNullOrEmpty(filterParameters.Username))
            {
                result = result.FindAll(user => user.Username.Contains(filterParameters.Username));
            }
            if (!string.IsNullOrEmpty(filterParameters.Email))
            {
                result = result.FindAll(user => user.Email.Contains(filterParameters.Email));
            }
            if (!string.IsNullOrEmpty(filterParameters.PhoneNumber))
            {
                result = result.FindAll(user => user.PhoneNumber.ToString().Contains(filterParameters.PhoneNumber));
            }

            if (!string.IsNullOrEmpty(filterParameters.SortBy))
            {
                if (filterParameters.SortBy.Equals("username", StringComparison.InvariantCultureIgnoreCase))
                {
                    result = result.OrderBy(user => user.Username).ToList();
                }
                else if (filterParameters.SortBy.Equals("email", StringComparison.InvariantCultureIgnoreCase))
                {
                    result = result.OrderBy(user => user.Email).ToList();
                }
                else if (filterParameters.SortBy.Equals("phoneNumber", StringComparison.InvariantCultureIgnoreCase))
                {
                    result = result.OrderBy(user => user.PhoneNumber).ToList();
                }

                if (!string.IsNullOrEmpty(filterParameters.SortOrder) && filterParameters.SortOrder.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                {
                    result.Reverse();
                }
            }

            return result;
        }
        public User GetById(int id)
        {
            User? user = context.Users
                .Where(users => users.Id == id)
                .FirstOrDefault();
            return user ?? throw new EntityNotFoundException($"User with ID = {id} doesn't exist.");
        }
        public User GetByUsername(string username)
        {
            User? user = context.Users
                .Where(users => users.Username == username)
                .FirstOrDefault();
            return user ?? throw new EntityNotFoundException($"User with username '{username}' doesn't exist.");
        }
        public User GetByEmail(string email)
        {
            User? user = context.Users
                .Where(users => users.Email == email)
                .FirstOrDefault();
            return user ?? throw new EntityNotFoundException($"User with email '{email}' doesn't exist.");
        }
        public User GetByPhoneNumber(string phoneNumber)
        {
            User? user = context.Users
                .Where(users => users.PhoneNumber == phoneNumber)
                .FirstOrDefault();
            return user ?? throw new EntityNotFoundException($"User with pnone number '{phoneNumber}' doesn't exist.");
        }
        //public PaginatedList<User> FilterBy(UserQueryParameters filterParameters)
        //{
        //    return this.repository.FilterBy(filterParameters);
        //}
        public User Create(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
            return user;
        }
        public User Update(int id, User user, User loggedUser)
        {
            User userToUpdate = this.GetById(id);

            userToUpdate.FirstName = user.FirstName ?? userToUpdate.FirstName;
            userToUpdate.LastName = user.LastName ?? userToUpdate.LastName;
            userToUpdate.Password = user.Password ?? userToUpdate.Password;
            userToUpdate.Email = user.Email ?? userToUpdate.Email;
            userToUpdate.Username = user.Username ?? userToUpdate.Username;
            userToUpdate.IsBlocked = user.IsBlocked;
            UpdatePhoneNumber(user, userToUpdate);
            UpdateAdminStatus(user, userToUpdate);

            context.SaveChanges();
            return userToUpdate;
        }
        public User Delete(int id)
        {
            User userToDelete = this.GetById(id);
            //userToDelete = context.Users
            //    .Include(u => u.Cards)
            //    .FirstOrDefault(u => u.Id == id);
            // context.Cards.RemoveRange(userToDelete.Cards);
            context.Users.Remove(userToDelete);
            context.SaveChanges();
            return userToDelete;
        }
        public User Promote(int id)
        {
            User userToPromote = this.GetById(id);
            if (!userToPromote.IsAdmin)
            {
                userToPromote.IsAdmin = true;
            }
            context.SaveChanges();
            return userToPromote;
        }
        public User BlockUser(int id)
        {
            User userToBlock = this.GetById(id);
            if (!userToBlock.IsBlocked)
            {
                userToBlock.IsBlocked = true;
            }
            context.SaveChanges();
            return userToBlock;
        }
        public User UnblockUser(int id)
        {
            User userToUnblock = this.GetById(id);
            if (userToUnblock.IsBlocked)
            {
                userToUnblock.IsBlocked = false;
            }
            context.SaveChanges();
            return userToUnblock;
        }
        public void UpdatePhoneNumber(User user, User userToUpdate)
        {
            if (user?.PhoneNumber != null)
            {
                userToUpdate.PhoneNumber = user.PhoneNumber;
            }
        }
        public void UpdateAdminStatus(User user, User userToUpdate)
        {
            if (!userToUpdate.IsAdmin)
            {
                userToUpdate.IsAdmin = user.IsAdmin;
            }
            else
            {
                userToUpdate.IsAdmin = true;
            }
        }
        public bool EmailExists(string email)
        {
            return context.Users.Any(u => u.Email == email);
        }
        public bool UsernameExists(string username)
        {
            return context.Users.Any(u => u.Username == username);
        }
        public bool PhoneNumberExists(string phoneNumber)
        {
            return context.Users.Any(u => u.PhoneNumber == phoneNumber);
        }
    }
}
