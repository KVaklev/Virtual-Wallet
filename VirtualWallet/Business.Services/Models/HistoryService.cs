using Business.DTOs.Responses;
using Business.Mappers;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;

namespace Business.Services.Models
{
    public class HistoryService : IHistoryService
    {
        private readonly IHistoryRepository historyRepository; 
        
        public HistoryService(IHistoryRepository historyRepository)
        {
            this.historyRepository = historyRepository;
        }

        public async Task<Response<PaginatedList<GetHistoryDto>>> FilterByAsync(
            HistoryQueryParameters filterParameters, 
            User loggedUser)
        {
            var result = new Response<PaginatedList<GetHistoryDto>>();

            if (filterParameters.Username != null && !loggedUser.IsAdmin)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }

            IQueryable<History> historyRecords = this.historyRepository.GetAll();

            if (!historyRecords.Any())
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }
            if (!loggedUser.IsAdmin)
            {
                historyRecords = historyRecords
                    .Where(u => u.AccountId == loggedUser.AccountId)
                    .AsQueryable();
            }

            historyRecords = await FilterByUsernameAsync(historyRecords, filterParameters.Username);
            historyRecords = await FilterByFromDataAsync(historyRecords, filterParameters.FromDate);
            historyRecords = await FilterByToDataAsync(historyRecords, filterParameters.ToDate);

            int totalPages = (historyRecords.Count() + filterParameters.PageSize - 1) / filterParameters.PageSize;
            historyRecords = await Common<History>.PaginateAsync(historyRecords, filterParameters.PageNumber, filterParameters.PageSize);

            if (!historyRecords.Any())
            {
               result.IsSuccessful = false;
               result.Message = Constants.NoRecordsFound;
               return result;
            }
            
            var resultDto = historyRecords
                             .Select(history => HistoryMapper.MapHistoryToDtoAsync(history)) 
                             .ToList();

            result.Data = new PaginatedList<GetHistoryDto>(resultDto, totalPages, filterParameters.PageNumber);
            
            return result; 
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
