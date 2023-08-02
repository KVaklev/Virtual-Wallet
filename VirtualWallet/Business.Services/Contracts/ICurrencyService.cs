using Business.Dto;
using Business.DTOs;
using DataAccess.Models.Models;
using System;
namespace Business.Services.Contracts
{
    public interface ICurrencyService
    {
        List<CurrencyDto> GetAll();
        Task<CurrencyDto> GetByIdAsync(int id);
        Task<Response<CurrencyDto>> CreateAsync(CurrencyDto currencyDto, User loggedUser);
        Task<Response<CurrencyDto>> UpdateAsync(int id, CurrencyDto currencyDto, User loggedUser);
        Task<Response<bool>> DeleteAsync(int id, User loggedUser);

    }
}
