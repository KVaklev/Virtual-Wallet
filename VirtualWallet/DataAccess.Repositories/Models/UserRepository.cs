using Business.Exceptions;
using Business.QueryParameters;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Models
{
    public class UserRepository : IUserRepository
    {

        private readonly ApplicationContext context;

        public UserRepository(ApplicationContext context)
        {
            this.context = context;
        }
        public async Task<List<User>> GetAllAsync()
        {
            return await context.Users
                .Where(u=>u.IsDeleted==false)
                .ToListAsync();
        }
        public async Task<PaginatedList<User>> FilterByAsync(UserQueryParameters filterParameters)
        {
            IQueryable<User> result = context.Users
                .Where(u => u.IsDeleted == false);

            result = await FilterByUsernameAsync(result, filterParameters.Username);
            result = await FilterByEmailAsync(result, filterParameters.Email);
            result = await FilterByPhoneNumberAsync(result, filterParameters.PhoneNumber);
            result = await SortByAsync(result, filterParameters.SortBy);
            result = await SortOrderAsync(result, filterParameters.SortOrder);

            int totalItems = await result.CountAsync();

            if (totalItems == 0)
            {
                throw new EntityNotFoundException("No users match the specified filter criteria.");
            }

            int totalPages = (result.Count() + filterParameters.PageSize - 1) / filterParameters.PageSize;

            result = await PaginateAsync(result, filterParameters.PageNumber, filterParameters.PageSize);

            return new PaginatedList<User>(result.ToList(), totalPages, filterParameters.PageNumber);

        }

        public async Task<User> GetByIdAsync(int id)
        {
            User? user = await context.Users
                .Where(u => u.IsDeleted == false)
                .Where(users => users.Id == id)
                .FirstOrDefaultAsync();
            return user ?? throw new EntityNotFoundException($"User with ID = {id} doesn't exist.");
        }
        public async Task<User> GetByUsernameAsync(string username)
        {
            User? user = await context.Users
                .Where(u => u.IsDeleted == false)
                .Include(u => u.Account)
                .Where(users => users.Username == username)
                .FirstOrDefaultAsync();
            return user ?? throw new EntityNotFoundException($"User with username '{username}' doesn't exist.");
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            User? user = await context.Users
                .Where(u => u.IsDeleted == false)
                .Where(users => users.Email == email)
                .FirstOrDefaultAsync();
            return user ?? throw new EntityNotFoundException($"User with email '{email}' doesn't exist.");
        }
        public async Task<User> GetByPhoneNumberAsync(string phoneNumber)
        {
            User? user = await context.Users
                .Where(u => u.IsDeleted == false)
                .Where(users => users.PhoneNumber == phoneNumber)
                .FirstOrDefaultAsync();
            return user ?? throw new EntityNotFoundException($"User with pnone number '{phoneNumber}' doesn't exist.");
        }
        public async Task<User> CreateAsync(User user)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }
        public async Task<User> UpdateAsync(int id, User user)
        {
            User userToUpdate = await this.GetByIdAsync(id);

            userToUpdate.FirstName = user.FirstName ?? userToUpdate.FirstName;
            userToUpdate.LastName = user.LastName ?? userToUpdate.LastName;
            userToUpdate.Password = user.Password ?? userToUpdate.Password;
            userToUpdate.Email = user.Email ?? userToUpdate.Email;
            userToUpdate.Username = user.Username ?? userToUpdate.Username;
            userToUpdate.IsBlocked = user.IsBlocked;
            await UpdatePhoneNumberAsync(user, userToUpdate);
            await UpdateAdminStatusAsync(user, userToUpdate);

            await context.SaveChangesAsync();
            return userToUpdate;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            User userToDelete = await this.GetByIdAsync(id);
            userToDelete.IsDeleted = true;

            await context.SaveChangesAsync();
            return userToDelete.IsDeleted;
        }
        public async Task<User> PromoteAsync(int id)
        {
            User userToPromote = await this.GetByIdAsync(id);
            if (!userToPromote.IsAdmin)
            {
                userToPromote.IsAdmin = true;
            }
            await context.SaveChangesAsync();
            return userToPromote;
        }
        public async Task<User> BlockUserAsync(int id)
        {
            User userToBlock = await this.GetByIdAsync(id);
            if (!userToBlock.IsBlocked)
            {
                userToBlock.IsBlocked = true;
            }
            await context.SaveChangesAsync();
            return userToBlock;
        }
        public async Task<User> UnblockUserAsync(int id)
        {
            User userToUnblock = await this.GetByIdAsync(id);
            if (userToUnblock.IsBlocked)
            {
                userToUnblock.IsBlocked = false;
            }
            await context.SaveChangesAsync();
            return userToUnblock;
        }
        public async Task UpdatePhoneNumberAsync(User user, User userToUpdate)
        {
            if (user?.PhoneNumber != null)
            {
                userToUpdate.PhoneNumber = user.PhoneNumber;
            }
        }
        public async Task UpdateAdminStatusAsync(User user, User userToUpdate)
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
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await context.Users.AnyAsync(u => u.Email == email);
        }
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await context.Users.AnyAsync(u => u.Username == username);
        }
        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
        {
            return await context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<IQueryable<User>> FilterByUsernameAsync(IQueryable<User> result, string? username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                result = result.Where(user => user.Username != null && user.Username == username);
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<User>> FilterByEmailAsync(IQueryable<User> result, string? email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                result = result.Where(user => user.Email != null && user.Email == email);
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<User>> FilterByPhoneNumberAsync(IQueryable<User> result, string? phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                result = result.Where(user => user.PhoneNumber != null && user.PhoneNumber == phoneNumber);
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<User>> SortByAsync(IQueryable<User> result, string? sortCriteria)
        {
            switch (sortCriteria)
            {
                case "username":
                    return await Task.FromResult(result.OrderBy(user => user.Username));
                case "email":
                    return await Task.FromResult(result.OrderBy(user => user.Email));
                case "phoneNumber":
                    return await Task.FromResult(result.OrderBy(user => user.PhoneNumber));
                default:
                    return await Task.FromResult(result);
            }
        }
        private async Task<IQueryable<User>> SortOrderAsync(IQueryable<User> result, string? sortOrder)
        {
            switch (sortOrder)
            {
                case "desc":
                    return await Task.FromResult(result.Reverse());
                default:
                    return await Task.FromResult(result);
            }
        }

        public async static Task<IQueryable<User>> PaginateAsync(IQueryable<User> result, int pageNumber, int pageSize)
        {
            return await Task.FromResult(result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize));
        }
    }


}
