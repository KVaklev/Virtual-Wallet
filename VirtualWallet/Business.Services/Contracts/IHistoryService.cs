using Business.DTOs;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface IHistoryService
    {
        Task<Response<GetHistoryDto>> GetByIdAsync(int id, User loggedUser);
        IQueryable<GetHistoryDto> GetAll(User loggedUser);
        Task<Response<IQueryable<GetHistoryDto>>> FilterByAsync(
            HistoryQueryParameters filterParameters,
            User loggedUser);
    }
}
