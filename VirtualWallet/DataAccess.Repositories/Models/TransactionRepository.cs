using Business.Exceptions;
using DataAccess.Models.Models;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using DataAccess.Repositories.Contracts;
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

        public Transaction CreateOutTransaction(Transaction transaction)
        {
            transaction.Date = DateTime.Now;
            transaction.IsExecuted = false;
            transaction.IsDeleted = false;
            context.Add(transaction);
            context.SaveChanges();

            return transaction;
        }

        public Transaction CreateInTransaction(Transaction transactionOut)
        {
            var transactionIn = new Transaction();
            transactionIn.AccountRecepientId = transactionOut.AccountRecepientId;
            transactionIn.AccountSenderId = transactionOut.AccountSenderId;
            transactionIn.CurrencyId = transactionOut.CurrencyId;
            transactionIn.Direction = DirectionType.In;
            transactionIn.Amount = transactionOut.Amount;
            transactionIn.Date = DateTime.Now;
            transactionIn.IsDeleted = false;
            transactionIn.IsExecuted = true;

            context.Add(transactionIn);
            context.SaveChanges();
            return transactionIn;
        }


        public Transaction GetById(int id) 
        {
            Transaction transaction = context.Transactions.Where(t => t.Id == id)
                .Include(s =>s.AccountSender)
                .Include(r =>r.AccountRecepient)
                .Include(c =>c.Currency)
                .FirstOrDefault();

            return transaction ?? throw new EntityNotFoundException("Transaction doesn't exist.");
        }

        public Transaction Update(Transaction transactionToUpdate, Transaction transaction)
        {
            transactionToUpdate.AccountRecepientId = transaction.AccountRecepientId;
            transactionToUpdate.Amount = transaction.Amount;
            transactionToUpdate.CurrencyId = transaction.CurrencyId;
            transactionToUpdate.Date = DateTime.Now;

            context.SaveChanges();
            return transactionToUpdate;
        }

        public bool Delete(Transaction transaction)
        {
            transaction.IsDeleted = true;
            context.SaveChanges();
            return transaction.IsDeleted;
        }

        public IQueryable<Transaction> GetAll(string username)
        {
            IQueryable<Transaction> result = context.Transactions
                    .Include(s => s.AccountSender)
                    .Include(r => r.AccountRecepient)
                    .ThenInclude(u =>u.User)
                    .Include(c => c.Currency);

            return result ?? throw new EntityNotFoundException("Тhere are no transactions!");
        }

        public PaginatedList<Transaction> FilterBy(TransactionQueryParameters filterParameters, string username)
        {
            IQueryable<Transaction> result = this.GetAll(username);

            result = FilterByRecipient(result, filterParameters.ResipientUsername);
            result = FilterByDirection(result, filterParameters.Direction);
            result = FilterByFromData(result, filterParameters.FromDate);
            result = FilterByToData(result, filterParameters.ToDate);
            result = SortBy(result, filterParameters.SortBy);

            //TODO check for user and in,out
            //if (result.Count == 0)
            //{
            //    throw new EntityNotFoundException(Constants.ModifyTransactionNoDataErrorMessage);
            //}
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
                return transactions.Where(t => t.AccountRecepient.User.Username == username);
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
