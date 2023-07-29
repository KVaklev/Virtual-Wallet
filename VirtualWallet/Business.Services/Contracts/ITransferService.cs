using Business.Dto;
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
        PaginatedList<Transfer> FilterBy(TransferQueryParameters transferQueryParameters, User user);
        Task <Transfer> GetByIdAsync(int id, User user);
        Task <Transfer> CreateAsync(Transfer transfer, User user);
        Task <Transfer> UpdateAsync(int id, Transfer transfer, User user);
        Task <bool> DeleteAsync(int id, User user);
        Task <bool> ExecuteAsync(int transferId, User user);


    }
}
