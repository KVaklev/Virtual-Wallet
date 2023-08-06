using Business.DTOs;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface ICardService
    {
        Response<IQueryable<GetCardDto>> GetAll();
        Task<Response<PaginatedList<GetCreatedCardDto>>> FilterByAsync(CardQueryParameters queryParameters);
        Task<Response<GetCardDto>> GetByIdAsync(int id, User loggedUser);
        Response<IQueryable<GetCardDto>> GetByAccountId(int accountId);
        Task<Response<GetCreatedCardDto>> CreateAsync(int accountId, CreateCardDto card);
        Task<Response<GetUpdatedCardDto>> UpdateAsync(int id, User loggedUser, UpdateCardDto card);
        Task<Response<bool>> DeleteAsync(int id, User loggedUser);
        Task<Response<Card>> IncreaseBalanceAsync(int id, decimal amount, User loggedUser);
        Task<Response<Card>> DecreaseBalanceAsync(int id, decimal amount, User loggedUser);
    }
}
