﻿using Business.DTOs;
using Business.Services.Contracts;
using DataAccess.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net.Http;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

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
                exchangeRateData.CurrencyValue = decimal.Parse(currencyValue, CultureInfo.InvariantCulture); ;

              
                result.IsSuccessful = true;
                result.Data = exchangeRateData;
                return result;
            }
            //todo
            catch (HttpRequestException ex)
            {  
                result.IsSuccessful = false;
                result.Message = "No such host is known.";
                return result;
            }
            catch (JsonException ex)
            {
                result.IsSuccessful = false;
                result.Message = "JSON Deserialization Error";
                return result;
            }
        }
        public async Task<decimal> ExchangeAsync(decimal amount, string fromCurrencyCode, string toCurrencyCode)
        {
            var exchangeRateDataResult = await GetExchangeRateDataAsync(fromCurrencyCode, toCurrencyCode);
            if (exchangeRateDataResult.IsSuccessful)
            {
                amount *= exchangeRateDataResult.Data.CurrencyValue;
            }
            return amount;
        }
    }
}

