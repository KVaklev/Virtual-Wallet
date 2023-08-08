
using Business.DTOs;
using Business.DTOs.Requests;
using DataAccess.Models.Models;
using System;
namespace Business.Services.Contracts
{
    public interface ICurrencyService
    {
        Response<IQueryable<CreateCurrencyDto>> GetAll();
        Task<Response<CreateCurrencyDto>> GetByIdAsync(int id);
        Task<Response<Currency>> GetCurrencyByIdAsync(int id);
        Task<Response<Currency>> GetByCurrencyCodeAsync(string id);
        Task<Response<CreateCurrencyDto>> CreateAsync(CreateCurrencyDto currencyDto, User loggedUser);
        Task<Response<CreateCurrencyDto>> UpdateAsync(int id, CreateCurrencyDto currencyDto, User loggedUser);
        Task<Response<bool>> DeleteAsync(int id, User loggedUser);

    }
}
