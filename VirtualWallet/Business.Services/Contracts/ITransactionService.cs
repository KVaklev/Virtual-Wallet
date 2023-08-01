using Business.Dto;
using Business.DTOs;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface ITransactionService
    {
        Task<GetTransactionDto> GetByIdAsync(int id, User loggedUser);
        Task<PaginatedList<Transaction>> FilterByAsync(TransactionQueryParameters filterParameters, User loggedUser);
        Task<Response<GetTransactionDto>> CreateAsync(CreateTransactionDto transactionDto, string loggedUser);
        Task<Transaction> UpdateAsync(int id, User user, CreateTransactionDto transactionDto);
        Task<bool> DeleteAsync(int id, User logedUser);
        Task<bool> ExecuteAsync(int transactionId, User user);
    }
}
