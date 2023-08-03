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
       
        public async Task<Response<CreateCurrencyDto>> CreateAsync(CreateCurrencyDto currencyDto, User loggedUser)
        {
            var result = new Response<CreateCurrencyDto>();
            if (!await Common.IsAdminAsync(loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyUserErrorMessage;
                return result;
            }
            var currency = this.mapper.Map<Currency>(currencyDto);
            var newCurrency = await this.currencyRepository.CreateAsync(currency);
            var newCurrencyDto = this.mapper.Map<CreateCurrencyDto>(currency);
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
            result.Message = Constants.ModifyCurrencyDeleteMessage;
            return result;
        }

        public IQueryable<CreateCurrencyDto> GetAll()
        {
           var currencies = this.currencyRepository.GetAll();
           var currenciesDto = currencies
                     .Select(currency => mapper.Map<CreateCurrencyDto>(currency))
                     .AsQueryable();

            return currenciesDto;
        }

        public async Task<CreateCurrencyDto> GetByIdAsync(int id)
        {
            var currency= await this.currencyRepository.GetByIdAsync(id);
            var currencyDto = this.mapper.Map<CreateCurrencyDto>(currency);
            return currencyDto;
        }

        public async Task<Response<CreateCurrencyDto>> UpdateAsync(int id, CreateCurrencyDto currencyDto, User loggedUser)
        {
            var result = new Response<CreateCurrencyDto>();
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
            var updatedCurrency = await CurrenciesMapper.MapUpdateAsync(currencyToUpdate, currency);
            this.currencyRepository.SaveChangesAsync();
            result.Data= this.mapper.Map<CreateCurrencyDto>(updatedCurrency); 
            return result;
        }

    }
}
