using Business.Dto;
using Business.DTOs;
using Business.QueryParameters;
using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Contracts
{
    public interface ITransferService
    {
        IQueryable <Transfer> GetAll(string username);
        Task<PaginatedList<Transfer>> FilterByAsync(TransferQueryParameters transferQueryParameters, User username);
        Task <GetTransferDto> GetByIdAsync(int id, User user);
        Task <Transfer> CreateAsync(CreateTransferDto transferDto, User user);
        Task <Transfer> UpdateAsync(int id, CreateTransferDto transferDto, User user);
        Task <bool> DeleteAsync(int id, User user);
        Task <bool> ExecuteAsync(int transferId, User user);


    }
}
