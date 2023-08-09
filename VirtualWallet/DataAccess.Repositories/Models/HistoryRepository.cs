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

            return history;
        }

        public async Task<History> CreateAsync(History history)
        {
            await this.context.AddAsync(history);
            await this.context.SaveChangesAsync();
            return history;
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
                result = result.Where(t => t.AccountId==loggedUser.AccountId);//TODO
            }
            return result;
        }

        public async Task<int> GetHistoryCountAsync()
        {
             var count = context.History.Count();
             return count;
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
