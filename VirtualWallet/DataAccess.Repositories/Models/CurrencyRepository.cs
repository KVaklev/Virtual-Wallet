using Business.Exceptions;
using DataAccess.Models.Models;
using DataAccess.Models.ValidationAttributes;
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
        public IQueryable<Currency> GetAll()
        {
            var currencies = this.context.Currencies
                .Where(c => c.IsDeleted == false)
                .OrderBy(n => n.Name)
                .AsQueryable();

            return currencies ?? throw new EntityNotFoundException(Constants.NoFoundErrorMessage);
        }

        public async Task<Currency> GetByIdAsync(int id)
        {
            var currency = await this.context.Currencies
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
            return currency ?? throw new EntityNotFoundException(Constants.NoFoundErrorMessage);
        }

        public async Task<Currency> CreateAsync(Currency currency)
        {
            context.Add(currency);
            await context.SaveChangesAsync();
            return currency;
        }

        public async Task<Currency> UpdateAsync(Currency currencyToUpdate, Currency currency)
        {
            currencyToUpdate.Name = currency.Name;
            currencyToUpdate.CurrencyCode = currency.CurrencyCode;
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

        public async Task<Currency> GetByCurrencyCodeAsync(string currencyCode)
        {
            var currency = await context.Currencies
                    .Where(c => c.CurrencyCode == currencyCode)
                    .FirstOrDefaultAsync();
 
            return currency ?? throw new EntityNotFoundException(Constants.NoFoundErrorMessage);
        }

    }
}
