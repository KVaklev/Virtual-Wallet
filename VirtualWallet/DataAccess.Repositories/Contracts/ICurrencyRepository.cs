using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Contracts
{
    public interface ICurrencyRepository
    {

        Currency Create(Currency currency);

        Currency GetById(int id);

        Currency Update(int id, Currency currency);

        bool Delete(int id);

        List<Currency> GetAll();
    }
}
