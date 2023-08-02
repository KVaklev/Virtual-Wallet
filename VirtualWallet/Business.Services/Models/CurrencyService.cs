using AutoMapper;
using Business.Dto;
using Business.DTOs;
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
       
        public async Task<Response<CurrencyDto>> CreateAsync(CurrencyDto currencyDto, User loggedUser)
        {
            var result = new Response<CurrencyDto>();
            if (!await Common.IsAdminAsync(loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyUserErrorMessage;
                return result;
            }
            var currency = this.mapper.Map<Currency>(currencyDto);
            var newCurrency = await this.currencyRepository.CreateAsync(currency);
            var newCurrencyDto = this.mapper.Map<CurrencyDto>(currency);
            result.Data = newCurrencyDto;
            return result;
        }

        public async Task<Response<bool>> DeleteAsync(int id, User loggedUser)
        {
            var result = new Response<bool>();
            if (!loggedUser.IsAdmin)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyUserErrorMessage;
                return result;

            }
            result.Data = await this.currencyRepository.DeleteAsync(id);
            result.Message = Constants.ModifyCurrencyDeleteErrorMessage;
            return result;
        }

        public List<CurrencyDto> GetAll()
        {
           var currencies = this.currencyRepository.GetAll();
           var currenciesDto = currencies
                     .Select(currency => mapper.Map<CurrencyDto>(currency))
                     .ToList();

            return currenciesDto;
        }

        public async Task<CurrencyDto> GetByIdAsync(int id)
        {
            var currency= await this.currencyRepository.GetByIdAsync(id);
            var currencyDto = this.mapper.Map<CurrencyDto>(currency);
            return currencyDto;
        }

        public async Task<Response<CurrencyDto>> UpdateAsync(int id, CurrencyDto currencyDto, User loggedUser)
        {
            var result = new Response<CurrencyDto>();
            if (!await Common.IsAdminAsync(loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyUserErrorMessage;
                return result;
            }

            var currencyToUpdate = await this.currencyRepository.GetByIdAsync(id);
            if (currencyToUpdate.IsDeleted)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyCurrencyNotFoundErrorMessage;
                return result;
            }

            var currency = this.mapper.Map<Currency>(currencyDto);
            var updatedCurrency = await this.currencyRepository.UpdateAsync(currencyToUpdate, currency);
            var updatedCurrencyDto = this.mapper.Map<CurrencyDto>(updatedCurrency);
            result.Data= updatedCurrencyDto;
            return result;
        }

    }
}
