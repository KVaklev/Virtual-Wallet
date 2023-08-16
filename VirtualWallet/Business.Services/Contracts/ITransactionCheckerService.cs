using Business.DTOs.Requests;
using Business.DTOs.Responses;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface ITransactionCheckerService
    {
        Task<Response<GetTransactionDto>> ChecksGetByIdAsync(Transaction transaction, User loggedUser);
        
        Task<Response<GetTransactionDto>> ChecksCreateOutTransactionAsync(
            CreateTransactionDto transactionDto,
            User loggedUser,
            Account recipient,
            Currency currency,
            Response<decimal> exchangeRate);

        Task<Response<GetTransactionDto>> ChecksUpdateAsync(
            Transaction transactionToUpdate,
            User loggedUser,
            CreateTransactionDto transactionDto,
            Account recipient,
            Currency currency,
            Response<decimal> exchangeRate);

        Task<Response<bool>> ChecksDeleteAsync(Transaction transaction, User loggedUser);
        
        Task<Response<bool>> ChecksConfirmAsync(Transaction transaction, User loggedUser);
    }
}
