using Business.Exceptions;
using Business.QueryParameters;
using DataAccess.Models.Enums;
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

        public IQueryable<Transfer> GetAll(string username)
        {
            IQueryable<Transfer> result = context.Transfers
                .Include(a => a.Account)
                .ThenInclude(u => u.User)
                .Include(t => t.Amount)
                .Include(c => c.Currency);

            result = result.Where(a => a.Account.User.Username == username);

            return result ?? throw new EntityNotFoundException("There are no transfers!");
        }

        public async Task<Transfer> CreateAsync(Transfer transfer)
        {
            transfer.DateCreated = DateTime.Now;
            transfer.IsConfirmed = false;
            transfer.IsCancelled = false;
            context.Add(transfer);
            await context.SaveChangesAsync();

            return transfer;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Transfer transferToDelete = await GetByIdAsync(id);
            transferToDelete.IsCancelled = true;
            context.Transfers.Remove(transferToDelete);
            await context.SaveChangesAsync();

            return transferToDelete.IsCancelled;
        }

        public async Task<Transfer> GetByIdAsync(int id)
        {
            Transfer transfer = await context.Transfers
                .Include(t => t.Account)
                .ThenInclude(u => u.User)
                .Include(t => t.Currency)
                .FirstOrDefaultAsync(t => t.Id == id);

            return transfer ?? throw new EntityNotFoundException($"Transfer with ID = {id} does not exist");

        }

        public async Task<Transfer> GetByUserIdAsync(int userId)
        {
            Transfer transfer = await context.Transfers
                .Include(a => a.Account)
                .ThenInclude(u => u.User)
                .FirstOrDefaultAsync(t => t.Account.User.Id == userId);

            return transfer ?? throw new EntityNotFoundException($"Transfer with UserId = {userId} does not exist");
        }

        public async Task<Transfer> UpdateAsync(int id, Transfer transfer)
        {
            var transferToUpdate = await GetByIdAsync(id);

            if (transferToUpdate.IsConfirmed)
            {
                throw new UnauthorizedOperationException("Transfer is confirmed! You are not authorized to modify it");
            }

            transferToUpdate.Amount = transfer.Amount;
            transferToUpdate.CardId = transfer.CardId;
            transferToUpdate.CurrencyId = transfer.CurrencyId;


            await context.SaveChangesAsync();
            return transferToUpdate;

        }


        public PaginatedList<Transfer> FilterBy(TransferQueryParameters filterParameters, string username)
        {
            
            IQueryable<Transfer> result = GetAll(username);

            result = FilterByUsername(result, filterParameters.Username);
            result = FilterByFromDate(result, filterParameters.FromDate);
            result = FilterByToDate(result, filterParameters.ToDate);
            result = FilterByTransferType(result, filterParameters.TransferType);
            result = SortBy(result, filterParameters.SortBy);

            int finalResult = result.Count();

            if (finalResult == 0)
            {
                throw new EntityNotFoundException("No results found by the specified filter criteria.");
            }

            int totalPages = (result.Count() + filterParameters.PageSize - 1) /
                filterParameters.PageSize;

            result = Paginate(result, filterParameters.PageNumber, filterParameters.PageSize);

            return new PaginatedList<Transfer>(result.ToList(), totalPages, filterParameters.PageNumber);


        }

        public static IQueryable<Transfer> Paginate(IQueryable<Transfer> result, int pageNumber, int pageSize)
        {
            return result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        private IQueryable<Transfer> FilterByUsername(IQueryable<Transfer> transfers, string? username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                transfers = transfers.Where(t => t.Account.User.Username == username);
            }

            return transfers;
        }
        private IQueryable<Transfer> FilterByFromDate(IQueryable<Transfer> transfers, string? fromDate)
        {
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime date = DateTime.Parse(fromDate);

                transfers = transfers.Where(a => a.DateCreated >= date);
            }

            return transfers;
        }
        private IQueryable<Transfer> FilterByToDate(IQueryable<Transfer> transfers, string? toDate)
        {
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime date = DateTime.Parse(toDate);

                transfers = transfers.Where(a => a.DateCreated <= date);
            }

            return transfers;
        }
        private IQueryable<Transfer> FilterByTransferType(IQueryable<Transfer> transfers, string? transfer)
        {
            if (!string.IsNullOrEmpty(transfer))
            {
                TransferDirection transferType = ParseTransferTypeParameter(transfer, "transfer");
                return transfers.Where(t => t.TransferType == transferType);
            }

            else
            {
                return transfers;
            }
        }

        private TransferDirection ParseTransferTypeParameter(string value, string parameter)
        {
            if (Enum.TryParse(value, true, out TransferDirection result))
            {
                return result;
            }

            throw new EntityNotFoundException($"Invalid value for {parameter}.");
        }

        private IQueryable<Transfer> SortBy(IQueryable<Transfer> transfers, string? sortCriteria)
        {
            switch (sortCriteria)
            {
                case "amount":
                    return transfers.OrderBy(t => t.Amount);
                case "date":
                    return transfers.OrderBy(t => t.DateCreated);
                default:
                    return transfers;

            }
        }
    }
}
