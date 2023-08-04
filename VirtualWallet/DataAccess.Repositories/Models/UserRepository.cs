using Business.Exceptions;
using Business.QueryParameters;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using DataAccess.Repositories.Helpers;
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

        public IQueryable<User> GetAll()
        {
            var users = context.Users
                .Where(u => u.IsDeleted == false)
                .Include(u=>u.Account)
                .ThenInclude(c=>c.Currency)
                .AsQueryable();

            return users ?? throw new EntityNotFoundException("There are no users.");
        }

        public async Task<PaginatedList<User>> FilterByAsync(UserQueryParameters filterParameters)
        {
            IQueryable<User> result = this.GetAll();

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
            result = await Common<User>.PaginateAsync(result, filterParameters.PageNumber, filterParameters.PageSize);

            return new PaginatedList<User>(result.ToList(), totalPages, filterParameters.PageNumber);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            User? user = await context.Users
                .Where(u => u.IsDeleted == false)
                .Where(users => users.Id == id)
                .Include(u => u.Account)
                .ThenInclude(c => c.Currency)
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

        public async Task<User> CreateAsync(User user)
        {
            context.Users.Add(user);

            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User updatedUser)
        {
            await context.SaveChangesAsync();
            return updatedUser;
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

            await context.SaveChangesAsync();
            return userToPromote;
        }

        public async Task<User> BlockUserAsync(int id)
        {
            User userToBlock = await this.GetByIdAsync(id);
          
            await context.SaveChangesAsync();
            return userToBlock;
        }

        public async Task<User> UnblockUserAsync(int id)
        {
            User userToUnblock = await this.GetByIdAsync(id);

            await context.SaveChangesAsync();
            return userToUnblock;
        }

        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
        {
            return await context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await context.Users.AnyAsync(u => u.Email == email);
        }
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await context.Users.AnyAsync(u => u.Username == username);
        }
        private async Task<IQueryable<User>> FilterByUsernameAsync(IQueryable<User> result, string? username)
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
            if (Enum.TryParse<SortCriteria>(sortCriteria, true, out var sortEnum))

            switch (sortEnum)
            {
                case SortCriteria.Username:
                    return await Task.FromResult(result.OrderBy(user => user.Username));
                case SortCriteria.Email:
                    return await Task.FromResult(result.OrderBy(user => user.Email));
                case SortCriteria.PhoneNumber:
                    return await Task.FromResult(result.OrderBy(user => user.PhoneNumber));
                default:
                    return await Task.FromResult(result);
            }

            else
            {
                return await Task.FromResult(result);
            }
        }
        private async Task<IQueryable<User>> SortOrderAsync(IQueryable<User> result, string? sortOrder)
        {
            if(Enum.TryParse<SortCriteria>(sortOrder, true, out var sortEnum))
            switch (sortEnum)
            {
                case SortCriteria.Desc:
                    return await Task.FromResult(result.Reverse());
                default:
                    return await Task.FromResult(result);
            }
            else
            {
                return await Task.FromResult(result);
            }
        }

        //private async Task UpdateAdminStatusAsync(User user, User userToUpdate)
        //{
        //    if (!userToUpdate.IsAdmin)
        //    {
        //        await Task.FromResult(userToUpdate.IsAdmin = user.IsAdmin);
        //    }
        //    else
        //    {
        //        await Task.FromResult(userToUpdate.IsAdmin = true);
        //    }
        //}

    }
}
