using AutoMapper;
using Business.Dto;
using Business.Exceptions;
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
       
        public async Task<CurrencyDto> CreateAsync(CurrencyDto currencyDto, User loggedUser)
        {
            if (!await Common.IsAdminAsync(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }
            var currency = this.mapper.Map<Currency>(currencyDto);
            var newCurrency = await this.currencyRepository.CreateAsync(currency);
            var newCurrencyDto = this.mapper.Map<CurrencyDto>(currency);
            return newCurrencyDto;
        }

        public async Task<bool> DeleteAsync(int id, User loggedUser)
        {
            if (!await Common.IsAdminAsync(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }
            return await this.currencyRepository.DeleteAsync(id);
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

        public async Task<Currency> UpdateAsync(int id, CurrencyDto currencyDto, User loggedUser)
        {
            if (!await Common.IsAdminAsync(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }
            var currencyToUpdate = await this.currencyRepository.GetByIdAsync(id);
            if (currencyToUpdate.IsDeleted)
            {
                throw new EntityNotFoundException(Constants.ModifyCurrencyNotFoundErrorMessage);
            }
            var currency = this.mapper.Map<Currency>(currencyDto);
            var updatedCurrency = await this.currencyRepository.UpdateAsync(currencyToUpdate, currency);
            var updatedCurrencyDto = this.mapper.Map<CurrencyDto>(updatedCurrency);
            return 
        }

    }
}
