using Business.Exceptions;
using DataAccess.Models.Contracts;
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
        public List<IUser> GetAll()
        {
            return context.Users.ToList();
        }
        public IUser GetById(int id)
        {
            IUser? user = context.Users
                .Where(users => users.Id == id)
                .FirstOrDefault();
            return user ?? throw new EntityNotFoundException($"User with ID = {id} doesn't exist.");
        }
        public IUser GetByUsername(string username)
        {
            IUser? user = context.Users
                .Where(users => users.Username == username)
                .FirstOrDefault();
            return user ?? throw new EntityNotFoundException($"User with username '{username}' doesn't exist.");
        }
        public IUser GetByEmail(string email)
        {
            IUser? user = context.Users
                .Where(users => users.Email == email)
                .FirstOrDefault();
            return user ?? throw new EntityNotFoundException($"User with email '{email}' doesn't exist.");
        }
        public IUser GetByPhoneNumber(int phoneNumber)
        {
            IUser? user = context.Users
                .Where(users => users.PhoneNumber == phoneNumber)
                .FirstOrDefault();
            return user ?? throw new EntityNotFoundException($"User with pnone number '{phoneNumber}' doesn't exist.");
        }
        //public PaginatedList<User> FilterBy(UserQueryParameters filterParameters)
        //{
        //    return this.repository.FilterBy(filterParameters);
        //}
        public IUser Create(IUser user)
        {
            context.Users.Add(user);
            context.SaveChanges();
            return user;
        }
        public IUser Update(int id, IUser user, IUser loggedUser)
        {
            IUser userToUpdate = this.GetById(id);

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
        public IUser Delete(int id)
        {
            IUser userToDelete = this.GetById(id);
            //userToDelete = context.Users
            //    .Include(u => u.Cards)
            //    .FirstOrDefault(u => u.Id == id);
            // context.Cards.RemoveRange(userToDelete.Cards);
            context.Users.Remove(userToDelete);
            context.SaveChanges();
            return userToDelete;
        }
        public void UpdatePhoneNumber(IUser user, IUser userToUpdate)
        {
            if (user?.PhoneNumber != null)
            {
                userToUpdate.PhoneNumber = user.PhoneNumber;
            }
        }
        public void UpdateAdminStatus(IUser user, IUser userToUpdate)
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
        public IUser Promote(IUser user)
        {
            if (!user.IsAdmin)
            {
                user.IsAdmin = true;
            }
            context.SaveChanges();
            return user;
        }
        public IUser BlockUser(IUser user)
        {
            if (!user.IsBlocked)
            {
                user.IsBlocked = true;
            }
            context.SaveChanges();
            return user;
        }
        public IUser UnblockUser(IUser user)
        {
            if (user.IsBlocked)
            {
                user.IsBlocked = false;
            }
            context.SaveChanges();
            return user;
        }
        public bool EmailExists(string email)
        {
            return context.Users.Any(u => u.Email == email);
        }
        public bool UsernameExists(string username)
        {
            return context.Users.Any(u => u.Username == username);
        }
        public bool PhoneNumberExists(int phoneNumber)
        {
            return context.Users.Any(u => u.PhoneNumber == phoneNumber);
        }
    }
}
