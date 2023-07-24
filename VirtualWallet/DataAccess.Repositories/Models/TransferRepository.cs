using Business.Exceptions;
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

        public IQueryable<Transfer> GetAll()
        {
            IQueryable<Transfer> result = context.Transfers
                .Include(a => a.Account)
                .ThenInclude(u=> u.User)
                .Include(t => t.Amount);
                
                
            return result ?? throw new EntityNotFoundException("There are no transfers!");
        }

        public Transfer Create(Transfer transfer)
        {
            transfer.Date = DateTime.Now;
            transfer.IsTransferred = false;
            context.Add(transfer);
            context.SaveChanges();

            return transfer;
        }
            
        public Transfer Delete(int id)
        {
            Transfer transferToDelete = this.GetById(id);
            context.Transfers .Remove(transferToDelete);
            context.SaveChanges();

            return transferToDelete;
        }
            

        public Transfer GetById(int id)
        {
            Transfer transfer = context.Transfers
                .Include(t => t.Account)
                .ThenInclude(u => u.User)
                .Include(t => t.Currency)
                .FirstOrDefault(t => t.Id == id);

            return transfer ?? throw new EntityNotFoundException($"Transfer with ID = {id} does not exist");

        }

        public Transfer GetByUserId(int userId)
        {
            Transfer transfer = context.Transfers
                .Include(a => a.Account)
                .ThenInclude(u => u.User)
                .FirstOrDefault(t => t.Account.User.Id == userId);

            return transfer ?? throw new EntityNotFoundException($"Transfer with UserId = {userId} does not exist");
        }

        public Transfer Update(int id, Transfer transfer)
        {
            var transferToUpdate = GetById(id);
            transferToUpdate.Amount = transfer.Amount;
            

            context.SaveChanges();
            return transferToUpdate;

        }

       
    }
}
