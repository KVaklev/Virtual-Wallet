using Business.DTOs;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface ICardService
    {
        Task<Response<PaginatedList<GetCreatedCardDto>>> FilterByAsync(CardQueryParameters queryParameters, User loggedUser);
        Task<Response<GetCardDto>> GetByIdAsync(int id, User loggedUser);
        Response<List<GetCardDto>> GetByAccountId(int accountId);
        Task<Response<GetCreatedCardDto>> CreateAsync(int accountId, CreateCardDto card);
        Task<Response<GetUpdatedCardDto>> UpdateAsync(int id, User loggedUser, UpdateCardDto card);
        Task<Response<bool>> DeleteAsync(int id, User loggedUser);
        Task<Response<Card>> IncreaseBalanceAsync(int id, decimal amount, User loggedUser);
        Task<Response<Card>> DecreaseBalanceAsync(int id, decimal amount, User loggedUser);
    }
}
