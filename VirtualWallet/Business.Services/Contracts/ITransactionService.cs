using Business.Dto;
using Business.DTOs;
using Business.QueryParameters;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface ITransactionService
    {
        Transaction Create(CreateTransactionDto transactionDto, User user);
        GetTransactionDto GetById(int id, User user);
        Transaction Update(int id, User user, CreateTransactionDto transactionDto);
        bool Delete(int id, User user);
        PaginatedList<Transaction> FilterBy(TransactionQueryParameters filterParameters, User user);
        public bool Execute(int transactionId, User user);
    }
}
