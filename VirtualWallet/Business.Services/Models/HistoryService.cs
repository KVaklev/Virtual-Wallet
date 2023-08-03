
using Business.DTOs;
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






        public async Task<Response<GetHistoryDto>> GetByIdAsync(int id, User loggedUser)
        {
            var result = new Response<GetHistoryDto>();
            var history = await historyRepository.GetByIdAsync(id);
            if (!await Common.IsHistoryOwnerAsync(history, loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }
            
            var historyDto = await HistoryMapper.MapHistoryToDtoAsync(history);
            result.Data = historyDto;
            return result;
        }

        public async Task<Response<IQueryable<GetHistoryDto>>> FilterByAsync(
            HistoryQueryParameters filterParameters, 
            User loggedUser)
        {
            var result = new Response<IQueryable<GetHistoryDto>>();
            if (filterParameters.Username != null && !loggedUser.IsAdmin)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }
            
            var historyRecords = await this.historyRepository.FilterByAsync(filterParameters, loggedUser);
            var resultDto = historyRecords
                             .Select(history => HistoryMapper.MapHistoryToDtoAsync(history)) 
                             .AsQueryable();
            result.Data = (IQueryable<GetHistoryDto>)resultDto;
            return result; // todo - PaginatedList<GetHistoryDto>
           
        }

    }
}
