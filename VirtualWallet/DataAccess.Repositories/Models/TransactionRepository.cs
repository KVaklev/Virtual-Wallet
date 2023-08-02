using Business.Exceptions;
using DataAccess.Models.Models;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using DataAccess.Repositories.Contracts;
using DataAccess.Models.Enums;
using Business.QueryParameters;
using DataAccess.Repositories.Helpers;

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
            Transaction transaction = await context.Transactions.Where(t => t.Id == id)
                .Include(s => s.AccountSender)
                .Include(r => r.AccountRecepient)
                .Include(c => c.Currency)
                .FirstOrDefaultAsync();

            //todo - filter by username
            return transaction ?? throw new EntityNotFoundException($"Transaction with ID = {id} doesn't exist.");
        }

        public IQueryable<Transaction> GetAll(string username)
        {
            IQueryable<Transaction> result = context.Transactions
                    .Include(s => s.AccountSender)
                    .Include(r => r.AccountRecepient)
                    .ThenInclude(u => u.User)
                    .Include(c => c.Currency)
                    .AsQueryable();

            //todo - filter by username
            return result ?? throw new EntityNotFoundException("Тhere are no transactions!");
        }
        public async Task<PaginatedList<Transaction>> FilterByAsync(TransactionQueryParameters filterParameters, string username)
        {
            IQueryable<Transaction> result = this.GetAll(username);

            result = await FilterByRecipientAsync(result, filterParameters.ResipientUsername);
            result = await FilterByDirectionAsync(result, filterParameters.Direction);
            result = await FilterByFromDataAsync(result, filterParameters.FromDate);
            result = await FilterByToDataAsync(result, filterParameters.ToDate);
            result = await SortByAsync(result, filterParameters.SortBy);

            int totalItems = await result.CountAsync();

            if (totalItems == 0)
            {
                throw new EntityNotFoundException("No transactions match the specified filter criteria.");
            }

            int totalPages = (result.Count() + filterParameters.PageSize - 1) / filterParameters.PageSize;
            result = await Common<Transaction>.PaginateAsync(result, filterParameters.PageNumber, filterParameters.PageSize);

            return new PaginatedList<Transaction>(result.ToList(), totalPages, filterParameters.PageNumber);
        }

        public async Task<Transaction> UpdateAsync(Transaction transactionToUpdate, Transaction transaction)
        {
            transactionToUpdate.AccountRecepientId = transaction.AccountRecepientId;
            transactionToUpdate.Amount = transaction.Amount;
            transactionToUpdate.CurrencyId = transaction.CurrencyId;
            transactionToUpdate.Date = DateTime.Now;

            await context.SaveChangesAsync();
            return transactionToUpdate;
        }

        public async Task<bool> DeleteAsync(Transaction transaction)
        {
            transaction.IsDeleted = true;
            await context.SaveChangesAsync();
            return transaction.IsDeleted;
        }

        public async Task<Transaction> CreateInTransactionAsync(Transaction transactionOut, decimal amount)
        {
            var transactionIn = new Transaction();
            transactionIn.AccountRecepientId = transactionOut.AccountRecepientId;
            transactionIn.AccountSenderId = transactionOut.AccountSenderId;
            transactionIn.Amount = amount;
            transactionIn.CurrencyId = (int)transactionOut.AccountRecepient.CurrencyId;
            transactionIn.Direction = DirectionType.In;
            transactionIn.Date = DateTime.Now;
            transactionIn.IsDeleted = false;
            transactionIn.IsExecuted = true;

            await context.AddAsync(transactionIn);
            await context.SaveChangesAsync();
            return transactionIn;
        }

        public async Task<Transaction> CreateOutTransactionAsync(Transaction transaction)
        {
            transaction.Date = DateTime.Now;
            transaction.IsExecuted = false;
            transaction.IsDeleted = false;

            await context.AddAsync(transaction);
            await context.SaveChangesAsync();

            return transaction;
        }

        private async Task<IQueryable<Transaction>> FilterByRecipientAsync(IQueryable<Transaction> result, string? username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                return result.Where(t => t.AccountRecepient.User.Username == username);
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<Transaction>> FilterByFromDataAsync(IQueryable<Transaction> result, string? fromData)
        {
            if (!string.IsNullOrEmpty(fromData))
            {
                DateTime date = DateTime.Parse(fromData);
                return result.Where(t => t.Date >= date);
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<Transaction>> FilterByToDataAsync(IQueryable<Transaction> result, string? toData)
        {
            if (!string.IsNullOrEmpty(toData))
            {
                DateTime date = DateTime.Parse(toData);

                return result.Where(t => t.Date <= date);
            }
            return await Task.FromResult(result);
        }
        private async Task<DirectionType> ParseDirectionParameterAsync(string value, string parameterName)
        {
            if (Enum.TryParse(value, true, out DirectionType result))
            {
                return await Task.FromResult(result);
            }

            throw new EntityNotFoundException($"Invalid value for {parameterName}.");
        }
        private async Task<IQueryable<Transaction>> FilterByDirectionAsync(IQueryable<Transaction> result, string? direction)
        {
            if (!string.IsNullOrEmpty(direction))
            {
                DirectionType directionType = await ParseDirectionParameterAsync(direction, "direction");
                return await Task.FromResult(result.Where(t => t.Direction == directionType));
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<Transaction>> SortByAsync(IQueryable<Transaction> result, string sortCriteria)
        {
            if (Enum.TryParse<SortCriteria>(sortCriteria, true, out var sortEnum))
            {
                switch (sortEnum)
                {
                    case SortCriteria.Amount:
                        return await Task.FromResult(result.OrderBy(t => t.Amount));
                    case SortCriteria.Date:
                        return await Task.FromResult(result.OrderBy(t => t.Date));
                    default:
                        return await Task.FromResult(result);
                }

            }
            else
            {
                return await Task.FromResult(result);   
            }

        }
    }
}
