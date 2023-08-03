using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface ICardService
    {
        IQueryable<Card> GetAll();
        Task<PaginatedList<Card>> FilterByAsync(CardQueryParameters queryParameters);
        Task<Card> GetByIdAsync(int id, User loggedUser);
        IQueryable<Card> GetByAccountId(int accountId);
        Task<Card> CreateAsync(int accountId, Card card);
        Task<GetUpdatedCardDto> UpdateAsync(int id, User loggedUser, UpdateCardDto card);
        Task<bool> DeleteAsync(int id, User loggedUser);
        Task<Card> IncreaseBalanceAsync(int id, decimal amount, User loggedUser);
        Task<Card> DecreaseBalanceAsync(int id, decimal amount, User loggedUser);
        Task<bool> CardNumberExistsAsync(string cardNumber);
       
    }
}
