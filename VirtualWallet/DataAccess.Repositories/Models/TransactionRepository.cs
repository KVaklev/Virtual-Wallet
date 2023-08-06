using Business.Exceptions;
using DataAccess.Models.Models;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using DataAccess.Repositories.Contracts;
using DataAccess.Models.Enums;
using Business.QueryParameters;
using DataAccess.Repositories.Helpers;
using DataAccess.Models.ValidationAttributes;

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
                .Include(s =>s.AccountSender)
                .Include(r =>r.AccountRecipient)
                .ThenInclude(c=>c.Currency)
                .Include(c =>c.Currency)
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

        public async Task<PaginatedList<Transaction>> FilterByAsync(TransactionQueryParameters filterParameters, string username)
        {
            IQueryable<Transaction> result = this.GetAll(username);

            result = await FilterByRecipientAsync(result, filterParameters.ResipientUsername);
            result = await FilterByDirectionAsync(result, filterParameters.Direction);
            result = await FilterByFromDataAsync(result, filterParameters.FromDate);
            result = await FilterByToDataAsync(result, filterParameters.ToDate);
            result = await SortByAsync(result, filterParameters.SortBy);

            int totalItems = await result.CountAsync();

            int totalPages = (result.Count() + filterParameters.PageSize - 1) / filterParameters.PageSize;
            result = await Common<Transaction>.PaginateAsync(result, filterParameters.PageNumber, filterParameters.PageSize);

            return new PaginatedList<Transaction>(result.ToList(), totalPages, filterParameters.PageNumber);
        }
        
        private IQueryable<Transaction> GetAll(string username)
        {
            IQueryable<Transaction> result = context.Transactions
                    .Where(u => u.AccountSender.User.Username == username)
                    .Include(s => s.AccountSender)
                    .Include(r => r.AccountRecipient)
                    .ThenInclude(u =>u.User)
                    .Include(c => c.Currency)
                    .AsQueryable();

            //todo - filter by username
            return result;
        }

        private async Task<IQueryable<Transaction>> FilterByRecipientAsync(IQueryable<Transaction> result, string? username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                return result.Where(t => t.AccountRecipient.User.Username == username);
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
            return result;
        }
        private async Task<IQueryable<Transaction>> FilterByToDataAsync(IQueryable<Transaction> result, string? toData)
        {
            if (!string.IsNullOrEmpty(toData))
            {
                DateTime date = DateTime.Parse(toData);

                return result.Where(t => t.Date <= date);
            }
            return result;
        }
        
        private async Task<IQueryable<Transaction>> FilterByDirectionAsync(IQueryable<Transaction> result, string? direction)
        {
            if (!string.IsNullOrEmpty(direction))
            {
                if (Enum.TryParse<DirectionType>(direction, true, out var directionEnum))
                {
                    DirectionType directionType = directionEnum;
                    return await Task.FromResult(result.Where(t => t.Direction == directionType));
                }
            }
            return result;
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
                return result;   
            }

        }
    }
}
