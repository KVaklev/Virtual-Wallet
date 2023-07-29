using Business.DTOs;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface IHistoryService
    {
        GetHistoryDto GetById(int id, User user);

         List<GetHistoryDto> FilterBy(HistoryQueryParameters filterParameters, User user);
    }
}
