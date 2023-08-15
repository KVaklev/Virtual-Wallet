using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.QueryParameters;
using DataAccess.Models.Models;


namespace Business.Services.Contracts
{
    public interface ITransferService
    {
        Task<Response<PaginatedList<GetTransferDto>>> FilterByAsync(TransferQueryParameters transferQueryParameters, User user);
        Task<Response<GetTransferDto>> GetByIdAsync(int id, User user);
        Task<Response<GetTransferDto>> CreateAsync(CreateTransferDto transferDto, User user);
        Task<Response<GetTransferDto>> UpdateAsync(int id, UpdateTransferDto transferDto, User user);
        Task<Response<bool>> DeleteAsync(int id, User user);
        Task<Response<bool>> ConfirmAsync(int transferId, User user);
    }
}
