using Business.Exceptions;
using DataAccess.Models.Models;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using DataAccess.Repositories.Contracts;
using System.Text;
using DataAccess.Models.Enums;
using Business.QueryParameters;

namespace DataAccess.Repositories.Models
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationContext context;

        public TransactionRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public Transaction Create(Transaction trasaction)
        {
            trasaction.Date = DateTime.Now;
            trasaction.IsExecuted = false;
            context.Add(trasaction);
            context.SaveChanges();

            return trasaction;
        }
        
        public Transaction GetById(int id) 
        {
            Transaction transaction = context.Transactions
                .Include(s =>s.Sender)
                .Include(r =>r.Recipient)
                .Include(c =>c.Currency)
                .FirstOrDefault(t => t.Id == id);

            return transaction ?? throw new EntityNotFoundException($"Transaction with ID = {id} doesn't exist.");
        }

        public Transaction Update(int id, Transaction transaction)
        {
            var transactionToUpdate = this.GetById(id);
            transactionToUpdate.RecipientId = transaction.RecipientId;
            transactionToUpdate.Amount = transaction.Amount;
            transactionToUpdate.CurrencyId = transaction.CurrencyId;
            transactionToUpdate.Date = transaction.Date;

            context.SaveChanges();
            return transactionToUpdate;
        }

        public bool Delete(int id)
        {
            var trasactions = this.GetById(id);
            trasactions.IsDeleted = true;

            return trasactions.IsDeleted;
        }

        public IQueryable<Transaction> GetAll()
        {
            IQueryable<Transaction> result = context.Transactions
                    .Include(s =>s.Sender)
                    .Include(r =>r.Recipient)
                    .Include(c => c.Currency);

            return result ?? throw new EntityNotFoundException("There are any transactions!");
        }

        public PaginatedList<Transaction> FilterBy(TransactionQueryParameters filterParameters)
        {
            IQueryable<Transaction> result = this.GetAll();

            result = FilterByRecipient(result, filterParameters.ResipientUsername);
            result = FilterByDirection(result, filterParameters.Direction);
            result = FilterByFromData(result, filterParameters.FromDate);
            result = FilterByToData(result, filterParameters.ToDate);
            result = SortBy(result, filterParameters.SortBy);

            int totalPages = (result.Count() + filterParameters.PageSize - 1) / filterParameters.PageSize;

            
            result = Paginate(result, filterParameters.PageNumber, filterParameters.PageSize);

            return new PaginatedList<Transaction>(result.ToList(), totalPages, filterParameters.PageNumber);
        }

        public static IQueryable<Transaction> Paginate(IQueryable<Transaction> result, int pageNumber, int pageSize)
        {
            return result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        private IQueryable<Transaction> FilterByRecipient(IQueryable<Transaction> transactions, string? username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                return transactions.Where(t => t.Recipient.Username == username);
            }
            else
            {
                return transactions;
            }
        }

        private IQueryable<Transaction> FilterByFromData(IQueryable<Transaction> transactions, string? fromData)
        {
            if (!string.IsNullOrEmpty(fromData))
            {
                DateTime date = DateTime.Parse(fromData);

                return transactions.Where(t => t.Date >= date);
            }
            else
            {
                return transactions;
            }
        }

        private IQueryable<Transaction> FilterByToData(IQueryable<Transaction> transactions, string? toData)
        {
            if (!string.IsNullOrEmpty(toData))
            {
                DateTime date = DateTime.Parse(toData);

                return transactions.Where(t => t.Date <= date);
            }
            else
            {
                return transactions;
            }
        }
        
        private DirectionType ParseDirectionParameter(string value, string parameterName)
        {
            if (Enum.TryParse(value, true, out DirectionType result))
            {
                return result;
            }
           //TODO
           throw new EntityNotFoundException($"Invalid value for {parameterName}.");
        }

        private IQueryable<Transaction> FilterByDirection(IQueryable<Transaction> transactions, string? direction)
        {
            if (!string.IsNullOrEmpty(direction))
            {
                DirectionType directionType = ParseDirectionParameter(direction, "direction");
                return transactions.Where(t => t.Direction == directionType);
            }
            else
            {
                return transactions;
            }
        }

        private IQueryable<Transaction> SortBy(IQueryable<Transaction> transactions, string sortCriteria)
        {
            switch (sortCriteria)
            {
                case "amount":
                    return transactions.OrderBy(t => t.Amount);
                case "date":
                    return transactions.OrderBy(t => t.Date);
                default:
                    return transactions;
            }
        }
    }
}
