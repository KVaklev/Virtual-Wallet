using AutoMapper;
using Business.DTOs;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Exceptions;
using Business.Mappers;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using DataAccess.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Models
{
    public class TransferService : ITransferService
    {
        private readonly ITransferRepository transferRepository;
        private readonly IHistoryRepository historyRepository;
        private readonly IAccountRepository accountRepository;
        private readonly IUserRepository userRepository;
        private readonly ICardRepository cardRepository;
        private readonly ApplicationContext context;
        private readonly IMapper mapper;
        private readonly ICurrencyRepository currencyRepository;
        private readonly IAccountService accountService;
        private readonly ICardService cardService;
        private readonly IExchangeRateService exchangeRateService;

        public TransferService(
            ITransferRepository transferRepository,
            IHistoryRepository historyRepository,
            IAccountRepository accountRepository,
            IUserRepository userRepository,
            ICardRepository cardRepository,
            ApplicationContext context,
            IMapper mapper,
            ICurrencyRepository currencyRepository,
            IAccountService accountService,
            ICardService cardService,
            IExchangeRateService exchangeRateService)
        {
            this.transferRepository = transferRepository;
            this.historyRepository = historyRepository;
            this.accountRepository = accountRepository;
            this.userRepository = userRepository;
            this.context = context;
            this.mapper = mapper;
            this.currencyRepository = currencyRepository;
            this.cardRepository = cardRepository;
            this.accountService = accountService;
            this.cardService = cardService;
            this.exchangeRateService = exchangeRateService;
        }

        public IQueryable<Transfer> GetAll(User user)
        {
            return transferRepository.GetAll(user);

        }

        public async Task<Response<GetTransferDto>> GetByIdAsync(int id, User user)
        {
            var result = new Response<GetTransferDto>();

            Transfer transferToGet = await transferRepository.GetByIdAsync(id);

            if (!await Security.IsUserAuthorizedAsync(transferToGet, user) || user.IsAdmin)

            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransferGetByIdErrorMessage;
                return result;
            }

            var transferDto = this.mapper.Map<GetTransferDto>(transferToGet);
            result.Data = transferDto;

            return result;
        }

        //public async Task<Transfer> CreateAsync(CreateTransferDto transferDto, User user)
        //{
        //    var card = this.cardRepository.GetByAccountId((int)user.AccountId).FirstOrDefault(x => x.CardNumber == transferDto.CardNumber);

        //    var transfer = await MapDtoToTransferAsync(transferDto, user, card);

        //    if (user.IsBlocked)
        //    {
        //        throw new UnauthorizedOperationException(Constants.ModifyTransferErrorMessage);
        //    }


        //    if (!await this.accountRepository.HasEnoughBalanceAsync(transfer.AccountId, transfer.Amount))
        //    {
        //        throw new UnauthorizedOperationException(Constants.ModifyAccountBalancetErrorMessage);
        //    }

        //    var createdTransfer = await this.transferRepository.CreateAsync(transfer);

        //    return createdTransfer;
        //}

        public async Task<Response<GetTransferDto>> CreateAsync(CreateTransferDto transferDto, User user)
        {

            var result = new Response<GetTransferDto>();


            if (user.IsBlocked)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransferErrorMessage;
                return result;
            }

            var card = this.cardRepository.GetByAccountId((int)user.AccountId).FirstOrDefault(x => x.CardNumber == transferDto.CardNumber);

            var currency = await this.currencyRepository.GetByCurrencyCodeAsync(transferDto.CurrencyCode);

            var transferType = Enum.Parse<TransferDirection>(transferDto.TransferType, true);
            
            var transfer = await TransfersMapper.MapCreateDtoToTransferAsync(transferDto, user, card, currency, transferType);


            if (transfer.TransferType == TransferDirection.Deposit)
            {
                if (!await Security.HasEnoughCardBalanceAsync(transfer.Card, transfer.Amount))
                {
                    result.IsSuccessful = false;
                    result.Message = Constants.ModifyAccountBalancetErrorMessage;

                    return result;

                }
            }
            else
            {
                if (!await Security.HasEnoughBalanceAsync(transfer.Account, transfer.Amount))
                {
                    result.IsSuccessful = false;
                    result.Message = Constants.ModifyAccountBalancetErrorMessage;

                    return result;

                }
            }
           

            transfer.DateCreated = DateTime.UtcNow;
            transfer.IsConfirmed = false;
            transfer.IsCancelled = false;

            var newTransfer =
            await this.transferRepository.CreateAsync(transfer);

            result.Data = this.mapper.Map<GetTransferDto>(newTransfer);

            return result;
        }


        public async Task<Response<bool>> DeleteAsync(int id, User user)
        {
            var result = new Response<bool>();

            Transfer transferToDelete = await transferRepository.GetByIdAsync(id);

            if (!await Security.IsUserAuthorizedAsync(transferToDelete, user))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransferErrorMessage;
                return result;

            }


            if (transferToDelete.IsCancelled || transferToDelete.IsConfirmed)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransferUpdateDeleteErrorErrorMessage;
                return result;

            }

            result.Message = Constants.ModifyTransferErrorMessage;
            result.Data = await this.transferRepository.DeleteAsync(id);
            return result;
        }


        public async Task<Response<GetTransferDto>> UpdateAsync(int id, UpdateTransferDto transferDto, User user)
        {
            var result = new Response<GetTransferDto>();

            Transfer transfer = await transferRepository.GetByIdAsync(id);

            if (!await Security.IsUserAuthorizedAsync(transfer, user))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransferErrorMessage;
                return result;
            }

            if (!await Security.HasEnoughBalanceAsync(transfer.Account, transfer.Account.Balance))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAccountBalancetErrorMessage;
                return result;
            }

            if (transfer.IsConfirmed || transfer.IsCancelled)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransferUpdateDeleteErrorErrorMessage;
                return result;
            }

            var card = this.cardRepository.GetByAccountId((int)user.AccountId).FirstOrDefault(x => x.CardNumber == transferDto.CardNumber);

            var currency = await this.currencyRepository.GetByCurrencyCodeAsync(transferDto.CurrencyCode);

            await TransfersMapper.MapUpdateDtoToTransferAsync(transferDto, user, card, currency);

            await this.transferRepository.SaveChangesAsync();

            result.Data = this.mapper.Map<GetTransferDto>(transfer);

            result.IsSuccessful = true;

            return result;
        }


        public async Task<Response<List<GetTransferDto>>> FilterByAsync(TransferQueryParameters transferQueryParameters, User loggedUser)
        {
            var transfers = await this.transferRepository.FilterByAsync(transferQueryParameters, loggedUser);

            var result = new Response<List<GetTransferDto>>();

            if (transfers.Count == 0)
            {
                throw new EntityNotFoundException(Constants.ModifyTransferNoDataErrorMessage);
            }

            List<GetTransferDto> transferDtos = transfers.Select(transfer => mapper.Map<GetTransferDto>(transfer)).ToList();


            result.Data = transferDtos;

            return result;

        }

        public async Task<Response<bool>> ExecuteAsync(int transferId, User user)
        {
            var result = new Response<bool>();

            Transfer transferToExecute = await transferRepository.GetByIdAsync(transferId);

            if (!(await Security.IsUserAuthorizedAsync(transferToExecute, user)))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransferErrorMessage;
                return result;
            }

            if (transferToExecute.IsConfirmed || transferToExecute.IsCancelled)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransferUpdateDeleteErrorErrorMessage;
                return result;
            }


            transferToExecute.IsConfirmed = true;
            transferToExecute.DateCreated = DateTime.UtcNow;

            await UpdateAccountsBalanceAsync(transferToExecute, user);

            await AddTransferToHistoryAsync(transferToExecute);

            await transferRepository.SaveChangesAsync();

            result.Message = Constants.ModifyExecutedTransfer;
            result.Data = transferToExecute.IsConfirmed;
            return result;

        }


        private async Task<decimal> GetCorrectAmountAsync(string transferCurrencyCode, string accountCurrencyCode, decimal amount)
        {
            if (transferCurrencyCode != accountCurrencyCode)
            {
                amount = await this.exchangeRateService.ExchangeAsync(amount, transferCurrencyCode, accountCurrencyCode);
            }
            return amount;
        }

        private async Task<bool> UpdateAccountsBalanceAsync(Transfer transfer, User user)
        {
            var accountAmount = await GetCorrectAmountAsync(transfer.Currency.CurrencyCode, transfer.Account.Currency.CurrencyCode, transfer.Amount);

            var cardAmount = await GetCorrectAmountAsync(transfer.Currency.CurrencyCode, transfer.Card.Currency.CurrencyCode, transfer.Amount);

            if (transfer.TransferType == TransferDirection.Deposit)
            {

                await this.accountService.IncreaseBalanceAsync(transfer.AccountId, accountAmount, user);

                await this.cardService.DecreaseBalanceAsync(transfer.CardId, cardAmount, user);
            }

            if (transfer.TransferType == TransferDirection.Withdrawal)
            {
                await this.accountService.DecreaseBalanceAsync(transfer.AccountId, accountAmount, user);

                await this.cardService.IncreaseBalanceAsync(transfer.CardId, cardAmount, user);
            }

            return true;
        }



        private async Task<bool> AddTransferToHistoryAsync(Transfer transfer)
        {
            var historyCount = await this.context.History.CountAsync();

            var history = await HistoryMapper.MapCreateWithTransferAsync(transfer);
            int historyCountNewHistoryAdded = await this.context.History.CountAsync();

            if (historyCount + 1 == historyCountNewHistoryAdded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }




    }


}
