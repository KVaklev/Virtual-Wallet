using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Models
{
    public class TransferRepository : ITransferRepository
    {
        private readonly ApplicationContext context;

        public TransferRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public IQueryable<Transfer> GetAll(User user)
        {
            IQueryable<Transfer> result = context.Transfers
            .Include(a => a.Account)
            .ThenInclude(u => u.User)
            .Include(c => c.Currency)
            .Include(t => t.Card);

            return result;
        }

        public async Task<Transfer> CreateAsync(Transfer transfer)
        {
            //transfer.DateCreated = DateTime.Now;
            //transfer.IsConfirmed = false;
            //transfer.IsCancelled = false;
            await context.AddAsync(transfer);
            await context.SaveChangesAsync();

            return transfer;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Transfer transferToDelete = await GetByIdAsync(id);
            transferToDelete.IsCancelled = true;
            //context.Transfers.Remove(transferToDelete);
            await context.SaveChangesAsync();

            return transferToDelete.IsCancelled;
        }

        public async Task<bool> SaveChangesAsync()
        {
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Transfer> GetByIdAsync(int id)
        {
            Transfer? transfer = await context.Transfers
                .Include(t => t.Account)
                .ThenInclude(t => t.Currency)
                 .Include(t => t.Account)
                .ThenInclude(u => u.User)
                .Include(t => t.Card)
                .ThenInclude(c => c.Currency)
                .Include(t => t.Currency)
                .FirstOrDefaultAsync(t => t.Id == id);

            return transfer;

        }

        public async Task<Transfer> GetByUserIdAsync(int userId)
        {
            var transfer = await context.Transfers
                .Include(a => a.Account)
                .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(t => t.Account.User.Id == userId);

            return transfer;

        }
        public async Task<Transfer> UpdateAsync(Transfer transferToUpdate)
        {
            await context.SaveChangesAsync();
            return transferToUpdate;
        }
    }
}
