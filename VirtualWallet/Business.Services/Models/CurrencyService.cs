using AutoMapper;
using Business.DTOs;
using Business.DTOs.Requests;
using Business.Mappers;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;

namespace Business.Services.Models
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository currencyRepository;
        private readonly IMapper mapper;

        public CurrencyService(
            ICurrencyRepository currencyRepository,
            IMapper mapper
      )
        {
            this.currencyRepository = currencyRepository;
            this.mapper = mapper;
        }

        public async Task<Response<Currency>> CreateAsync(CreateCurrencyDto currencyDto, User loggedUser)
        {
            var result = new Response<Currency>();
            if (!loggedUser.IsAdmin)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyUserErrorMessage;
                return result;
            }
            var currency = this.mapper.Map<Currency>(currencyDto);
            var newCurrency = await this.currencyRepository.CreateAsync(currency); 
            result.Data = newCurrency;
            return result;
        }

        public async Task<Response<Currency>> DeleteAsync(int id, User loggedUser)
        {
            var result = new Response<Currency>();

            if (!loggedUser.IsAdmin)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyUserErrorMessage;
                return result;

            }
            var currencyExists = await this.currencyRepository.GetByIdAsync(id);

            if (currencyExists == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.CurrencyNotFoundErrorMessage;
                return result;
            }
                currencyExists.IsDeleted = true;
                await this.currencyRepository.SaveChangesAsync();
                result.Data = currencyExists;
            
            return result;
        }

        public async Task<Response<List<Currency>>> GetAllAsync()
        {
            var result = new Response<List<Currency>>();

            var currencies = this.currencyRepository.GetAll();

            if(!currencies.Any())
            {
                result.IsSuccessful = false;
                result.Message = Constants.NotFoundResults;
;
                return result;
            }
            result.Data = currencies.ToList();

            return result;
        }

        public async Task<Response<Currency>> GetByCurrencyCodeAsync(string currencyCode)
        {
            var result = new Response<Currency>();

            Currency currencyToGet = await currencyRepository.GetByCurrencyCodeAsync(currencyCode);

            if(currencyToGet == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyNoRecordsFound;
                return result;
            }

            result.Data = currencyToGet;
            return result;
        }

       

        public async Task<Response<Currency>> GetCurrencyByIdAsync(int id)
        {
            var result = new Response<Currency>();

            var currency = await this.currencyRepository.GetByIdAsync(id);

            if (currency == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyNoRecordsFound;
                return result;
            }
           
            result.Data = currency;
            return result;
        }

        public async Task<Response<Currency>> UpdateAsync(int id, CreateCurrencyDto currencyDto, User loggedUser)
        {
            var result = new Response<Currency>();

            if (!loggedUser.IsAdmin)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyUserErrorMessage;
                return result;
            }

            var currencyToUpdate = await this.currencyRepository.GetByIdAsync(id);
            if (currencyToUpdate == null || currencyToUpdate.IsDeleted)
            {
                result.IsSuccessful = false;
                result.Message = Constants.CurrencyNotFoundErrorMessage;
                return result;
            }

            var currency = this.mapper.Map<Currency>(currencyDto);
            var updatedCurrency = await CurrenciesMapper.MapUpdateAsync(currencyToUpdate, currency);
            this.currencyRepository.SaveChangesAsync();
            result.Data = updatedCurrency;
            return result;
        }

    }
}
