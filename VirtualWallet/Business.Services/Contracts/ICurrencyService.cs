
using Business.DTOs;
using Business.DTOs.Requests;
using DataAccess.Models.Models;
using System;
namespace Business.Services.Contracts
{
    public interface ICurrencyService
    {
        IQueryable<CreateCurrencyDto> GetAll();
        Task<CreateCurrencyDto> GetByIdAsync(int id);
        Task<Currency> GetByCurrencyCodeAsync(string id);
        Task<Response<CreateCurrencyDto>> CreateAsync(CreateCurrencyDto currencyDto, User loggedUser);
        Task<Response<CreateCurrencyDto>> UpdateAsync(int id, CreateCurrencyDto currencyDto, User loggedUser);
        Task<Response<bool>> DeleteAsync(int id, User loggedUser);

    }
}
