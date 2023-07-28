using Business.Exceptions;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;

namespace Business.Services.Models
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository currencyRepository;

        public CurrencyService(ICurrencyRepository currencyRepository)
        {
            this.currencyRepository = currencyRepository;
        }
       
        public async Task<Currency> CreateAsync(Currency currency, User loggedUser)
        {
            if (!await Common.IsAdminAsync(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }
            return await this.currencyRepository.CreateAsync(currency);
        }

        public async Task<bool> DeleteAsync(int id, User loggedUser)
        {
            if (!await Common.IsAdminAsync(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }
            return await this.currencyRepository.DeleteAsync(id);
        }

        public async Task<List<Currency>> GetAllAsync()
        {
            return await this.currencyRepository.GetAllAsync();
        }

        public async Task<Currency> GetByIdAsync(int id)
        {
            return await this.currencyRepository.GetByIdAsync(id);
        }

        public async Task<Currency> UpdateAsync(int id, Currency currency, User loggedUser)
        {
            if (!await Common.IsAdminAsync(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }
            return await this.currencyRepository.UpdateAsync(id, currency);
        }

        public async Task<Currency> GetByАbbreviationAsync(string abbreviation)
        {
            return await this.currencyRepository.GetByАbbreviationAsync(abbreviation);
        }

    }
}
