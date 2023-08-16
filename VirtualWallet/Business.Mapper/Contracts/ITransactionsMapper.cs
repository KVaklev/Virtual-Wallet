using Business.DTOs.Requests;
using Business.DTOs.Responses;
using DataAccess.Models.Models;

namespace Business.Mappers.Contracts
{
    public interface ITransactionsMapper
    {
        Task<Transaction> MapOutToInTransactionAsync(
            Transaction transactionOut,
            decimal amountExchange,
            decimal exchangeRate);

        Task<Transaction> MapUpdateDtoToTransactionAsync(
            Transaction transactionToUpdate,
            CreateTransactionDto transactionDto,
            Account recipient,
            Currency currency,
            decimal exchangeRate);

        Task<Transaction> MapDtoТоTransactionAsync(
            CreateTransactionDto transactionDto,
            User user,
            Account account,
            Currency currency,
            decimal exchangeRate);

        Task<CreateTransactionDto> MapGetDtoToCreateDto(GetTransactionDto getTransactionDto);

    }
}
