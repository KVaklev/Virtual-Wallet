using Business.DTOs;
using Business.DTOs.Responses;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface IAccountService
    {
        Response<IQueryable<GetAccountDto>> GetAll();
        
        Task<Response<GetAccountDto>> GetByIdAsync(int id, User user);
        
        Task<Response<GetAccountDto>> GetByUsernameAsync(int id, User user);
        
        Task<Response<GetAccountDto>> CreateAsync(string currencyCode, User user);
        
        Task<Response<bool>> AddCardAsync(int id, Card card, User user);
        
        Task<Response<bool>> RemoveCardAsync(int id,  Card card, User user);
        
        Task<Response<bool>> DeleteAsync(int id, User loggedUser);
        
        Task<Response<Account>> IncreaseBalanceAsync(int id, decimal amount, User loggedUser);
        
        Task<Response<Account>> DecreaseBalanceAsync(int id, decimal amount, User loggedUser);
        
        Task<Response<string>> GenerateTokenAsync(int id);
        
        Task<Response<bool>> ConfirmRegistrationAsync(int id, string token);
    }
}
