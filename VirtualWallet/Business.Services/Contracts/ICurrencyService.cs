using Business.Dto;
using DataAccess.Models.Models;
using System;
namespace Business.Services.Contracts
{
    public interface ICurrencyService
    {
        IQueryable<Currency> GetAll();
        Task<Currency> GetByIdAsync(int id);
        Task<Currency> GetByАbbreviationAsync(string abbreviation);
        Task<CurrencyDto> CreateAsync(CurrencyDto currencyDto, User loggedUser);
        Task<Currency> UpdateAsync(int id, Currency currency, User loggedUser);
        Task<bool> DeleteAsync(int id, User loggedUser);

    }
}
