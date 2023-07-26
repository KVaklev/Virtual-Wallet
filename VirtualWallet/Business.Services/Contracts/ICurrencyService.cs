using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Contracts
{
    public interface ICurrencyService
    {
        Currency Create(Currency currency, User user);

        Currency GetById(int id);

        Currency Update(int id, Currency currency, User user);

        bool Delete(int id, User user);

        List<Currency> GetAll();

        Currency GetByАbbreviation(string abbreviation);
    }
}
