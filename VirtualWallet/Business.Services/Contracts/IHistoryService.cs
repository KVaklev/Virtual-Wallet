using Business.DTOs;
using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface IHistoryService
    {
        Task<Response<GetHistoryDto>> GetByIdAsync(int id, User loggedUser);
        Task<Response<PaginatedList<GetHistoryDto>>> FilterByAsync(HistoryQueryParameters filterParameters,User loggedUser);
    }
}
