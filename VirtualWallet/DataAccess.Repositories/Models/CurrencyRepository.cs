using Business.Exceptions;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Models
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly ApplicationContext context;

        public CurrencyRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public Currency Create(Currency currency)
        {
            currency.IsDeleted = false;
            context.Add(currency);
            context.SaveChanges();
            return currency;
        }

        public Currency GetById(int id)
        {
            var currency = this.context.Currencies.Where(c => c.Id == id).FirstOrDefault();
            return currency ?? throw new EntityNotFoundException("Тhere is no such currency");
        }

        public Currency Update(int id, Currency currency)
        {
            var currencyToUpdate = this.GetById(id);
            currencyToUpdate.Name = currency.Name;
            currencyToUpdate.Аbbreviation = currency.Аbbreviation;
            context.SaveChanges();

            return currencyToUpdate;
        }

        public bool Delete(int id) 
        {
            var currencyToDelete = this.GetById(id);
            currencyToDelete.IsDeleted = true;
            context.SaveChanges();

            return currencyToDelete.IsDeleted;
        }

        public List<Currency> GetAll()
        {
            var currencies = context.Currencies
                .Where(c => c.IsDeleted == false)
                .OrderBy(n=>n.Name)
                .ToList();
            return currencies ?? throw new EntityNotFoundException("Тhere is no such currency");
        }

        public Currency GetByАbbreviation(string abbreviation)
        {
            var currency = context.Currencies
                    .Where(c => c.Аbbreviation == abbreviation)
                    .FirstOrDefault();
 
            return currency ?? throw new EntityNotFoundException("Тhere is no such currency");
        }

    }
}
