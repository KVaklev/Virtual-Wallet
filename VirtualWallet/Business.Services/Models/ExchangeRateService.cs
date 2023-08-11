
using Business.Services.Contracts;
using Business.Services.Helpers;
using Newtonsoft.Json;
using System.Globalization;


namespace DataAccess.Models.Models
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient httpClient;
        
        public ExchangeRateService() 
        {
            this.httpClient = new HttpClient();
            this.httpClient.BaseAddress = new Uri("https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/");
        }

        public async Task<Response<ExchangeRate>> GetExchangeRateDataAsync(string fromCurrencyCode, string toCurrencyCode)
        {
            var result = new Response<ExchangeRate>();

            try
            {
                string endOfUrl = fromCurrencyCode.ToLower() + "/" + toCurrencyCode.ToLower() + ".json";
                HttpResponseMessage response = await httpClient.GetAsync(endOfUrl);//eur/usd.json
                string jsonResponse = await response.Content.ReadAsStringAsync();
                ExchangeRate exchangeRateData = JsonConvert.DeserializeObject<ExchangeRate>(jsonResponse);

                string currencyValue = jsonResponse.Substring(39, 8).ToString();
                if (currencyValue == null)
                {
                    result.IsSuccessful = false;
                    result.Message = Constants.NotFoundResults;
                    return result;
                }
                exchangeRateData.CurrencyValue = decimal.Parse(currencyValue, CultureInfo.InvariantCulture); ;

                result.Data = exchangeRateData;
                return result;
            }
            catch (HttpRequestException)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoHostError;
                return result;
            }
            catch (JsonException)
            {
                result.IsSuccessful = false;
                result.Message = Constants.JsonDeserializationError;
                return result;
            }
            catch (ArgumentOutOfRangeException)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ArgumentOutOfRangeError;
                return result;
            }
        }
        public async Task<Response<decimal>> ExchangeAsync(decimal amount, string fromCurrencyCode, string toCurrencyCode)
        {
            var result = new Response<decimal>();
            if (fromCurrencyCode == toCurrencyCode)
            {
                result.Data = amount;
            }
            else
            {
                var exchangeRateDataResult = await GetExchangeRateDataAsync(fromCurrencyCode, toCurrencyCode);
                if (!exchangeRateDataResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.Message = exchangeRateDataResult.Message;
                    return result;
                }
                result.Data = amount *= exchangeRateDataResult.Data.CurrencyValue;
            }
            return result;
        }
    }
}

