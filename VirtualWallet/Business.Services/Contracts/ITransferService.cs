using Business.DTOs;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
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
        IQueryable <Transfer> GetAll(User user);
        Task<Response<List<GetTransferDto>>> FilterByAsync(TransferQueryParameters transferQueryParameters, User user);
        Task<Response<GetTransferDto>> GetByIdAsync(int id, User user);
        Task <Response<GetTransferDto>> CreateAsync(CreateTransferDto transferDto, User user);
        Task<Response<GetTransferDto>> UpdateAsync(int id, UpdateTransferDto transferDto, User user);
        Task<Response<bool>> DeleteAsync(int id, User user);
        Task<Response<bool>> ExecuteAsync(int transferId, User user);


    }
}
