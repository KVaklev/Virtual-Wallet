using Business.DTOs;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface ITransactionService
    {
        Task<Response<GetTransactionDto>> GetByIdAsync(int id, User loggedUser);
       
        Task<Response<PaginatedList<GetTransactionDto>>> FilterByAsync(
            TransactionQueryParameters filterParameters,
            User loggedUser);
        
        Task<Response<GetTransactionDto>> CreateOutTransactionAsync(CreateTransactionDto transactionDto, User loggedUser);
        
        Task<Response<GetTransactionDto>> UpdateAsync(int id, User loggedUser, CreateTransactionDto transactionDto);
        
        Task<Response<bool>> DeleteAsync(int id, User loggedUser);
        
        Task<Response<bool>> ConfirmAsync(int transactionId, User loggedUser);
    }
}
