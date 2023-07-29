using DataAccess.Models.Models;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using Business.Exceptions;
using Business.QueryParameters;
using IHistoryRepository = DataAccess.Repositories.Contracts.IHistoryRepository;
using DataAccess.Models.Enums;

namespace DataAccess.Repositories.Models
{
    public class HistoryRepository: IHistoryRepository
    {
        private readonly ApplicationContext context;

        public HistoryRepository(ApplicationContext context) 
        {
             this.context=context;
        }

        public History CreateWithTransaction(Transaction transaction)
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
            this.context.Add(history);
            this.context.SaveChanges();

            return history;
        }
        
        public History CreateWithTransfer(Transfer transfer)
        {
            var history = new History();
            
            history.EventTime = DateTime.Now;
            history.TransferId = transfer.Id;
            history.NameOperation = NameOperation.Transfer;
            history.AccountId = transfer.AccountId;

            this.context.Add(history);
            this.context.SaveChanges();
            return history;
        }

        public History GetById(int id)
        {
            var history = context.History
                .Include(tr=>tr.Transaction)
                .Include(tf=>tf.Transfer)
                .Include(ac=>ac.Account)
                .ThenInclude(u=>u.User)
                .FirstOrDefault(h => h.Id == id);

            return history ?? throw new EntityNotFoundException($"No records with this id={id}");
        }

        public IQueryable<History> GetAll(User user)
        {
            IQueryable<History> result = context.History
                    .Include(tr => tr.Transaction)
                    .Include(tf => tf.Transfer)
                    .Include(ac => ac.Account)
                    .ThenInclude(u => u.User);

            if (!user.IsAdmin)
            { 
                result = result.Where(t => t.AccountId==user.AccountId);
            }

            return result ?? throw new EntityNotFoundException("Тhere are no records!");
        }

        public PaginatedList<History> FilterBy(HistoryQueryParameters filterParameters, User user)
        {
            IQueryable<History> result = this.GetAll(user);

            result = FilterByUsername(result, filterParameters.Username);
            result = FilterByFromData(result, filterParameters.FromDate);
            result = FilterByToData(result, filterParameters.ToDate);
            

            int totalPages = (result.Count() + filterParameters.PageSize - 1) / filterParameters.PageSize;
            result = Paginate(result, filterParameters.PageNumber, filterParameters.PageSize);
            return new PaginatedList<History>(result.ToList(), totalPages, filterParameters.PageNumber);
        }

        public IQueryable<History> Paginate(IQueryable<History> result, int pageNumber, int pageSize)
        {
            return result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        private IQueryable<History> FilterByUsername(IQueryable<History> histories, string? username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                return histories.Where(h=>h.Account.User.Username == username);
            }
            else
            {
                return histories;
            }
        }

        private IQueryable<History> FilterByFromData(IQueryable<History> history, string? fromData)
        {
            if (!string.IsNullOrEmpty(fromData))
            {
                DateTime date = DateTime.Parse(fromData);

                return history.Where(h => h.EventTime >= date);
            }
            else
            {
                return history;
            }
        }

        private IQueryable<History> FilterByToData(IQueryable<History> history, string? toData)
        {
            if (!string.IsNullOrEmpty(toData))
            {
                DateTime date = DateTime.Parse(toData);

                return history.Where(t => t.EventTime <= date);
            }
            else
            {
                return history;
            }
        }

        
    }
}
