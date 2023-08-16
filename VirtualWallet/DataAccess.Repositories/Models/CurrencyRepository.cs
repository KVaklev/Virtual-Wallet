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
                .OrderBy(n => n.Name)
                .AsQueryable();

            return currencies;
        }

        public async Task<Currency> CreateAsync(Currency currency)
        {
            await context.AddAsync(currency);
            await context.SaveChangesAsync();

            return currency;
        }

        public async Task<Currency> GetByIdAsync(int id)
        {
            var currency = await this.context.Currencies
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return currency;
        }

        public async Task<Currency> GetByCurrencyCodeAsync(string currencyCode)
        {
            var currency = await context.Currencies
                    .Where(c => c.CurrencyCode == currencyCode)
                    .FirstOrDefaultAsync();

            return currency;
        }

        public async Task<bool> SaveChangesAsync()
        {
            await context.SaveChangesAsync();

            return true;
        }
    }
}
