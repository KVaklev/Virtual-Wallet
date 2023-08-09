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

            return users;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            User? user = await context.Users
                .Where(u => u.IsDeleted == false)
                .Where(users => users.Id == id)
                .Include(u => u.Account)
                .ThenInclude(c => c.Currency)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            User? user = await context.Users
                .Where(u => u.IsDeleted == false)
                .Include(u => u.Account)
                .Where(users => users.Username == username)
                .FirstOrDefaultAsync();

            return user;
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
    }
}
