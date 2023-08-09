using Business.QueryParameters;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using DataAccess.Repositories.Helpers;
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


            if (!user.IsAdmin)
            {

                result = result.Where(a => a.Account.User.Username == user.Username);
            }


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
            Transfer transfer = await context.Transfers
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

        public async Task<PaginatedList<Transfer>> FilterByAsync(TransferQueryParameters filterParameters, User user)
        {

            IQueryable<Transfer> result = GetAll(user);

            result = await FilterByUsernameAsync(result, filterParameters.Username);
            result = await FilterByFromDateAsync(result, filterParameters.FromDate);
            result = await FilterByToDateAsync(result, filterParameters.ToDate);
            result = await FilterByTransferTypeAsync(result, filterParameters.TransferType);
            result = await SortByAsync(result, filterParameters.SortBy);

            int totalPages = (result.Count() + filterParameters.PageSize - 1) /
    filterParameters.PageSize;

            result = await Common<Transfer>.PaginateAsync(result, filterParameters.PageNumber, filterParameters.PageSize);

            return new PaginatedList<Transfer>(result.ToList(), totalPages, filterParameters.PageNumber);


        }

        private async Task<IQueryable<Transfer>> FilterByUsernameAsync(IQueryable<Transfer> transfers, string? username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                transfers = transfers.Where(t => t.Account.User.Username == username);
            }

            return await Task.FromResult(transfers);
        }
        private async Task<IQueryable<Transfer>> FilterByFromDateAsync(IQueryable<Transfer> transfers, string? fromDate)
        {
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime date = DateTime.Parse(fromDate);

                transfers = transfers.Where(a => a.DateCreated >= date);
            }

            return await Task.FromResult(transfers);
        }
        private async Task<IQueryable<Transfer>> FilterByToDateAsync(IQueryable<Transfer> transfers, string? toDate)
        {
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime date = DateTime.Parse(toDate);

                transfers = transfers.Where(a => a.DateCreated <= date);
            }

            return await Task.FromResult(transfers);
        }
        private async Task<IQueryable<Transfer>> FilterByTransferTypeAsync(IQueryable<Transfer> transfers, string? transfer)
        {
            if (!string.IsNullOrEmpty(transfer))
            {
                if (TryParseTransferTypeParameter(transfer, out TransferDirection? transferType))
                {
                    if (transferType.HasValue)
                    {
                        return await Task.FromResult(transfers.Where(t => t.TransferType == transferType.Value));
                    }
                    else
                    {
                        return await Task.FromResult(transfers);
                    }
                }
            }

            return await Task.FromResult(transfers);
        }

        private bool TryParseTransferTypeParameter(string value, out TransferDirection? result)
        {
            if (Enum.TryParse(value, true, out TransferDirection parsedResult))
            {
                result = parsedResult;
                return true;
            }

            result = null;
            return false;
        }

        public static async Task<IQueryable<Transfer>> SortByAsync(IQueryable<Transfer> transfers, string? sortCriteria)
        {
            if (Enum.TryParse<SortCriteria>(sortCriteria, true, out var sortEnum))
            {
                switch (sortEnum)
                {
                    case SortCriteria.Amount:
                        return await Task.FromResult(transfers.OrderBy(t => t.Amount));
                    case SortCriteria.Date:
                        return await Task.FromResult(transfers.OrderBy(t => t.DateCreated));

                    default:
                        return await Task.FromResult(transfers);
                }
            }
            else
            {
                return await Task.FromResult(transfers);

            }
        }

    }
}
