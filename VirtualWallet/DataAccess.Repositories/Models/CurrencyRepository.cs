﻿using Business.Exceptions;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Models
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly ApplicationContext context;

        public CurrencyRepository(ApplicationContext context)
        {
            this.context = context;
        }
        public async Task<List<Currency>> GetAllAsync()
        {
            var currencies = await this.context.Currencies
                .Where(c => c.IsDeleted == false)
                .OrderBy(n => n.Name)
                .ToListAsync();
            return currencies ?? throw new EntityNotFoundException("Тhere is no such currency");
        }

        public async Task<Currency> GetByIdAsync(int id)
        {
            var currency = await this.context.Currencies
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
            return currency ?? throw new EntityNotFoundException("Currency with ID = {id} doesn't exist.");
        }

        public async Task<Currency> CreateAsync(Currency currency)
        {
            currency.IsDeleted = false;
            context.Add(currency);
            await context.SaveChangesAsync();
            return currency;
        }

        public async Task<Currency> UpdateAsync(int id, Currency currency)
        {
            var currencyToUpdate = await this.GetByIdAsync(id);
            var currency = this.context.Currencies.Where(c => c.Id == id).FirstOrDefault();
            if (currency==null || currency.IsDeleted)
            {
                throw new EntityNotFoundException("Тhere is no such currency");
            }
            return currency; 
        }

        public Currency Update(int id, Currency currency)
        {
            var currencyToUpdate = this.GetById(id);
            currencyToUpdate.Name = currency.Name;
            currencyToUpdate.Abbreviation = currency.Abbreviation;
            await context.SaveChangesAsync();

            return currencyToUpdate;
        }

        public async Task<bool> DeleteAsync(int id) 
        {
            var currencyToDelete = await this.GetByIdAsync(id);
            currencyToDelete.IsDeleted = true;
            await context.SaveChangesAsync();

            return currencyToDelete.IsDeleted;
        }

        public async Task<Currency> GetByАbbreviationAsync(string abbreviation)
        {
            var currency = await context.Currencies
                    .Where(c => c.Abbreviation == abbreviation)
                    .FirstOrDefaultAsync();
 
            return currency ?? throw new EntityNotFoundException("Тhere is no such currency.");
        }

    }
}
