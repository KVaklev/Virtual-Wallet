using Business.Dto;
using DataAccess.Models.Models;
using System;
namespace Business.Services.Contracts
{
    public interface ICurrencyService
    {
        List<CurrencyDto> GetAll();
        Task<CurrencyDto> GetByIdAsync(int id);
        Task<CurrencyDto> CreateAsync(CurrencyDto currencyDto, User loggedUser);
        Task<CurrencyDto> UpdateAsync(int id, CurrencyDto currencyDto, User loggedUser);
        Task<bool> DeleteAsync(int id, User loggedUser);

    }
}
