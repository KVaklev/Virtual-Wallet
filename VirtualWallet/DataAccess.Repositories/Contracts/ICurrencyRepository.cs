using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface ICurrencyRepository
    {
        IQueryable<Currency> GetAll();
        Task<Currency> GetByIdAsync(int id);
        Task<Currency> GetByCurrencyCodeAsync(string abbreviation);
        Task<Currency> CreateAsync(Currency currency);
        Task<Currency> UpdateAsync(int id, Currency currency);
        Task<bool> DeleteAsync(int id);

    }
}
