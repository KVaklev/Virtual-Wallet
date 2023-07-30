using DataAccess.Models.Models;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using Business.Exceptions;
using Business.QueryParameters;
using DataAccess.Models.Enums;
using DataAccess.Repositories.Helpers;
using DataAccess.Repositories.Contracts;

namespace DataAccess.Repositories.Models
{
    public class HistoryRepository: IHistoryRepository
    {
        private readonly ApplicationContext context;

        public HistoryRepository(ApplicationContext context) 
        {
             this.context=context;
        }
        public IQueryable<History> GetAll(User loggedUser)
        {
            IQueryable<History> result = context.History
                .Include(tr => tr.Transaction)
                .ThenInclude(c => c.Currency)
                .Include(tf => tf.Transfer)
                .ThenInclude(c => c.Currency)
                .Include(ac => ac.Account)
                .ThenInclude(u => u.User);

            if (!loggedUser.IsAdmin)
            { 
                result = result.Where(t => t.AccountId==loggedUser.AccountId);
            }

            return result ?? throw new EntityNotFoundException("Тhere are no records!");
        }

        public async Task<History> GetByIdAsync(int id)
        {
            var history = await context.History
                .Include(tr=>tr.Transaction)
                .ThenInclude(c=>c.Currency)
                .Include(tf=>tf.Transfer)
                .ThenInclude(c => c.Currency)
                .Include(ac=>ac.Account)
                .ThenInclude(u=>u.User)
                .FirstOrDefaultAsync(h => h.Id == id);

            return history ?? throw new EntityNotFoundException($"There are no records for the specified id.");
        }

        public async Task<History> CreateWithTransactionAsync(Transaction transaction)
        {
            var history = new History();

            history.EventTime = DateTime.Now;
            history.TransactionId = transaction.Id;
            history.NameOperation = NameOperation.Transaction;

            if (transaction.Direction == DirectionType.Out)
            {
                history.AccountId = transaction.AccountSenderId;
            }
            else
            {
                history.AccountId = transaction.AccountRecepientId;
            }
            await this.context.AddAsync(history);
            await this.context.SaveChangesAsync();

            return history;
        }
        
        public async Task<History> CreateWithTransferAsync(Transfer transfer)
        {
            var history = new History();
            
            history.EventTime = DateTime.Now;
            history.TransferId = transfer.Id;
            history.NameOperation = NameOperation.Transfer;
            history.AccountId = transfer.AccountId;

            await this.context.AddAsync(history);
            await this.context.SaveChangesAsync();
            return history;
        }

        public async Task<PaginatedList<History>> FilterByAsync(HistoryQueryParameters filterParameters, User loggedUser)
        {
            IQueryable<History> result = this.GetAll(loggedUser);

            result = await FilterByUsernameAsync(result, filterParameters.Username);
            result = await FilterByFromDataAsync(result, filterParameters.FromDate);
            result = await FilterByToDataAsync(result, filterParameters.ToDate);

            int totalItems = await result.CountAsync();

            if (totalItems == 0)
            {
                throw new EntityNotFoundException("No history matches the specified filter criteria.");
            }

            int totalPages = (result.Count() + filterParameters.PageSize - 1) / filterParameters.PageSize;
            result = await Common<History>.PaginateAsync(result, filterParameters.PageNumber, filterParameters.PageSize);
           
            return new PaginatedList<History>(result.ToList(), totalPages, filterParameters.PageNumber);
        }

        private async Task<IQueryable<History>> FilterByUsernameAsync(IQueryable<History> result, string? username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                return result.Where(history => history.Account.User.Username == username);
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<History>> FilterByFromDataAsync(IQueryable<History> result, string? fromData)
        {
            if (!string.IsNullOrEmpty(fromData))
            {
                DateTime date = DateTime.Parse(fromData);

                return result.Where(history => history.EventTime >= date);
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<History>> FilterByToDataAsync(IQueryable<History> result, string? toData)
        {
            if (!string.IsNullOrEmpty(toData))
            {
                DateTime date = DateTime.Parse(toData);

                return result.Where(history => history.EventTime <= date);
            }
            return await Task.FromResult(result);
        }        
    }
}
