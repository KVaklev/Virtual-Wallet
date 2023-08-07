using Business.DTOs;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface IExchangeRateService
    {
        Task<Response<ExchangeRate>> GetExchangeRateDataAsync(string senderAccountCurrencyCode, string recepientAccountCurrencyCode);
        Task<Response<decimal>> ExchangeAsync(decimal amount, string fromCurrencyCode, string toCurrencyCode);
    }
}
