using Business.DTOs;
using DataAccess.Models.Models;

namespace Business.Services.Contracts
{
    public interface IExchangeRateService
    {
        Task<Response<ExchangeRateData>> GetExchangeRateDataAsync(string senderAccountCurrencyCode, string recepientAccountCurrencyCode);

        Task<decimal> ExchangeAsync(decimal amount, decimal currencyValue);
    }
}
