
using Business.DTOs;
using Business.DTOs.Requests;
using DataAccess.Models.Models;
using System;
namespace Business.Services.Contracts
{
    public interface ICurrencyService
    {
        Task<Response<List<Currency>>> GetAllAsync();
        Task<Response<Currency>> GetCurrencyByIdAsync(int id);
        Task<Response<Currency>> GetByCurrencyCodeAsync(string id);
        Task<Response<Currency>> CreateAsync(CreateCurrencyDto currencyDto, User loggedUser);
        Task<Response<Currency>> UpdateAsync(int id, CreateCurrencyDto currencyDto, User loggedUser);
        Task<Response<bool>> DeleteAsync(int id, User loggedUser);

    }
}
