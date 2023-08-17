using DataAccess.Models.Models;

namespace Business.ViewModels
{
    public class CurrencyViewModel
    {
        public List<Currency> Currencies {get; set;}
        public  User User { get; set;}
    }
}
