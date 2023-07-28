﻿using DataAccess.Models.Models;
using System;
namespace Business.Services.Contracts
{
    public interface ICurrencyService
    {
        Task<List<Currency>> GetAllAsync();
        Task<Currency> GetByIdAsync(int id);
        Task<Currency> GetByАbbreviationAsync(string abbreviation);
        Task<Currency> CreateAsync(Currency currency, User loggedUser);
        Task<Currency> UpdateAsync(int id, Currency currency, User loggedUser);
        Task<bool> DeleteAsync(int id, User loggedUser);

    }
}
