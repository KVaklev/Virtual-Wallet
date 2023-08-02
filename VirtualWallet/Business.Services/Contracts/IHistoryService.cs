using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface IHistoryService
    {
        Task<GetHistoryDto> GetByIdAsync(int id, User loggedUser);
        IQueryable<History> GetAll(User loggedUser);
        Task<List<GetHistoryDto>> FilterByAsync(HistoryQueryParameters filterParameters, User loggedUser);
    }
}
