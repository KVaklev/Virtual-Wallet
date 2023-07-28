using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface ICurrencyRepository
    {
        Task<List<Currency>> GetAllAsync();
        Task<Currency> GetByIdAsync(int id);
        Task<Currency> GetByАbbreviationAsync(string abbreviation);
        Task<Currency> CreateAsync(Currency currency);
        Task<Currency> UpdateAsync(int id, Currency currency);
        Task<bool> DeleteAsync(int id);

    }
}
