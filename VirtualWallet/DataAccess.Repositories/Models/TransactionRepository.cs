using DataAccess.Models.Models;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using DataAccess.Repositories.Contracts;

namespace DataAccess.Repositories.Models
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationContext context;

        public TransactionRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<Transaction> GetByIdAsync(int id) 
        {
            var transaction = await context.Transactions.Where(t => t.Id == id)
                .Include(s =>s.AccountSender)
                .ThenInclude((s => s.User))
                .Include(r => r.AccountSender)
                .ThenInclude(c => c.Currency)
                .Include(r =>r.AccountRecipient)
                .ThenInclude((s => s.User))
                .Include(r => r.AccountRecipient)
                .ThenInclude(c => c.Currency)
                .Include(c=>c.Currency)
                .FirstOrDefaultAsync();
            
            return transaction;
        }

        public async Task<bool> SaveChangesAsync()
        {
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Transaction transaction)
        {
            transaction.IsDeleted = true;
            await context.SaveChangesAsync();

            return transaction.IsDeleted;
        }

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        { 
            await context.AddAsync(transaction);
            await context.SaveChangesAsync();

            return transaction;
        }
        
        public IQueryable<Transaction> GetAll(string username)
        {
            var result = context.Transactions
                .Where(u => u.AccountSender.User.Username == username)
                .Include(s => s.AccountSender)
                .Include(r => r.AccountRecipient)
                .ThenInclude(u =>u.User)
                .Include(c => c.Currency)
                .OrderByDescending(d=>d.Date)
                .AsQueryable();

            return result;
        } 
    }
}
