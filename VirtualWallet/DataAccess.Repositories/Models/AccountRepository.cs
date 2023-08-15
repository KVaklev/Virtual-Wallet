using Business.Exceptions;
using DataAccess.Models.Models;
using DataAccess.Models.ValidationAttributes;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Models
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationContext context;
        private readonly IUserRepository userRepository;

        public AccountRepository(
            ApplicationContext context,  
            IUserRepository userRepository)
        {
            this.context = context;
            this.userRepository = userRepository;
        }

        public IQueryable <Account> GetAll()
        {
            IQueryable<Account> result = context.Accounts
                .Where(a => a.IsDeleted == false)
                .Include(a => a.User)
                .Include(a => a.Cards)
                .Include(a => a.Currency);

            return result;
        }
        public async Task <Account> GetByIdAsync(int id)
        {
            var account = await context.Accounts
                .Where(a => a.IsDeleted == false)
                .Where(a => a.Id == id)
                .Include(a => a.User)
                .Include(a => a.Cards)
                .Include(a => a.Currency)
                .FirstOrDefaultAsync();

            return account;
        }

        public async Task<Account> GetByUsernameAsync(string username)
        {
            var account = await context.Accounts
                .Include(a => a.User)
                .Include(a => a.Cards)
                .Include(a => a.Currency)
                .Where(a => a.IsDeleted == false)
                .Where(a => a.User.Username == username)
                .FirstOrDefaultAsync();

            return account;
        }

        public async Task<Account> CreateAsync(Account account)
        {
            context.Add(account);
            await context.SaveChangesAsync();

            return account;
        }

        public async Task <bool> DeleteAsync(int id)
        {
            var accountToDelete = await this.GetByIdAsync(id);
            accountToDelete.IsDeleted = true;
            await context.SaveChangesAsync();

            return accountToDelete.IsDeleted;
        }
               
        public async Task<bool> AddCardAsync(int id, Card card)
        {
            var accountToAddCard = await this.GetByIdAsync(id);       
            accountToAddCard.Cards.Add(card);
            await context.SaveChangesAsync();

            return await Task.FromResult(true);
        }

        public async Task <bool> RemoveCardAsync(int id, Card card)
        {
            var accountToRemoveCard = await this.GetByIdAsync(id);
            accountToRemoveCard.Cards.Remove(card);
            await context.SaveChangesAsync();

            return await Task.FromResult(true);
        }

        public async Task<bool> SaveChangesAsync()
        {
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmRegistrationAsync(int userId, string token)
        {
            var user = await this.userRepository.GetByIdAsync(userId);
            if (user==null)
            {
                return false;
            }
            user.IsVerified = true;
            await this.context.SaveChangesAsync();

            return true;
        }
    }
}
