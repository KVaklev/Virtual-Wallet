using DataAccess.Models.Models;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;
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
                result = result.Where(t => t.AccountId==loggedUser.AccountId);
            }

            return result;
        }       
    }
}
