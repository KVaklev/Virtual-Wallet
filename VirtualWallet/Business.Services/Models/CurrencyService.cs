using Business.Exceptions;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Models
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository currencyRepository;

            public CurrencyService(ICurrencyRepository currencyRepository)
        {
            this.currencyRepository = currencyRepository;
        }
       
        public Currency Create(Currency currency, User user)
        {
            if (!user.IsAdmin)
            {
                throw new UnauthorizedOperationException(Constants.ModifyCurrencyErrorMessage);
            }
            return this.currencyRepository.Create(currency);
        }

        public bool Delete(int id, User user)
        {
            if (!user.IsAdmin)
            {
                throw new UnauthorizedOperationException(Constants.ModifyCurrencyErrorMessage);
            }

            return this.currencyRepository.Delete(id);
        }

        public List<Currency> GetAll()
        {
            return this.currencyRepository.GetAll();
        }

        public Currency GetById(int id)
        {
            return this.currencyRepository.GetById(id);
        }

        public Currency Update(int id, Currency currency, User user)
        {
            if (!user.IsAdmin)
            {
                throw new UnauthorizedOperationException(Constants.ModifyCurrencyErrorMessage);
            }
            var currencyToUpdate = this.GetById(id);
            if (currencyToUpdate.IsDeleted)
            {
                throw new EntityNotFoundException(Constants.ModifyCurrencyNotFoundErrorMessage);
            }
            return this.currencyRepository.Update(id, currency);
        }

        public Currency GetByАbbreviation(string abbreviation)
        {
            return this.currencyRepository.GetByАbbreviation(abbreviation);
        }
    }
}
