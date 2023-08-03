﻿using AutoMapper;
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
        private readonly ICurrencyService currencyService;

        public TransferService(ITransferRepository transferRepository, IHistoryRepository historyRepository, IAccountRepository accountRepository, IUserRepository userRepository, ICardRepository cardRepository, ApplicationContext context, IMapper mapper, ICurrencyRepository currencyRepository, ICurrencyService currencyService )
        {
            this.transferRepository = transferRepository;
            this.historyRepository = historyRepository;
            this.accountRepository = accountRepository;
            this.userRepository = userRepository;
            this.context = context;
            this.mapper = mapper;
            this.currencyRepository = currencyRepository;
            this.cardRepository = cardRepository;
            this.currencyService = currencyService;
        }

        public IQueryable<Transfer> GetAll(User user)
        {
            return transferRepository.GetAll(user);
        }

        public async Task<GetTransferDto> GetByIdAsync(int id, User user)
        {
            Transfer transferToGet = await transferRepository.GetByIdAsync(id);

            if (!await Common.IsUserAuthorizedAsync(transferToGet, user) || user.IsAdmin)

            {
                throw new UnauthorizedOperationException(Constants.ModifyTransferGetByIdErrorMessage);
            }

            var transferDto = this.mapper.Map<GetTransferDto>(transferToGet);

            return transferDto;
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

        public async Task<Transfer> CreateAsync(CreateTransferDto transferDto, User user)
        {
            var card = this.cardRepository.GetByAccountId((int)user.AccountId).FirstOrDefault(x => x.CardNumber == transferDto.CardNumber);

            var currency = await this.currencyRepository.GetByCurrencyCodeAsync(transferDto.CurrencyCode);

            var transfer = await TransfersMapper.MapCreateDtoToTransferAsync(transferDto, user, card, currency);

            if (user.IsBlocked)
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransferErrorMessage);
            }

            if (!await this.accountRepository.HasEnoughBalanceAsync(transfer.AccountId, transfer.Amount))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountBalancetErrorMessage);
            }

            // Set additional properties before creating the entity
            transfer.DateCreated = DateTime.Now;
            transfer.IsConfirmed = false;
            transfer.IsCancelled = false;

            // Create the entity in the repository
            await this.transferRepository.CreateAsync(transfer);

            // Return the created transfer after it has been saved to the database
            return transfer;
        }


        public async Task<bool> DeleteAsync(int id, User user)
        {
            Transfer transferToDelete = await transferRepository.GetByIdAsync(id);

            if (!await Common.IsUserAuthorizedAsync(transferToDelete, user))
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransferErrorMessage);
            }


            if (transferToDelete.IsCancelled || transferToDelete.IsConfirmed)
            {
                throw new InvalidOperationException(Constants.ModifyTransferUpdateDeleteErrorErrorMessage);
            }
            return await this.transferRepository.DeleteAsync(id);
        }

        
        //public async Task<Transfer> UpdateAsync(int id, CreateTransferDto transferDto, User user)
        //{
        //    var card = this.cardRepository.GetByAccountId((int)user.AccountId).FirstOrDefault(x => x.CardNumber == transferDto.CardNumber);

        //    Transfer transferToUpdate = await transferRepository.GetByIdAsync(id);

        //    if (!await Common.IsUserAuthorizedAsync(transferToUpdate, user))
        //    {
        //        throw new UnauthorizedOperationException(Constants.ModifyTransferErrorMessage);
        //    }

        //    if (transferToUpdate.IsConfirmed || transferToUpdate.IsCancelled)
        //    {
        //        throw new UnauthorizedOperationException(Constants.ModifyTransferUpdateDeleteErrorErrorMessage);
        //    }

        //    var updatedTransfer = await MapDtoToTransferAsync(transferDto, user, card);

        //    return await this.transferRepository.UpdateAsync(transferToUpdate.Id, updatedTransfer);
        //}

        public async Task<Transfer> UpdateAsync(int id, UpdateTransferDto transferDto, User user)
        {
            var card = this.cardRepository.GetByAccountId((int)user.AccountId).FirstOrDefault(x => x.CardNumber == transferDto.CardNumber);

            var currency = await this.currencyRepository.GetByCurrencyCodeAsync(transferDto.CurrencyCode);

          
            Transfer transferToUpdate = await transferRepository.GetByIdAsync(id);

            transferToUpdate = await TransfersMapper.MapUpdateDtoToTransferAsync(transferDto, user, card, currency);

            if (!await Common.IsUserAuthorizedAsync(transferToUpdate, user))
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransferErrorMessage);
            }

            if (transferToUpdate.IsConfirmed || transferToUpdate.IsCancelled)
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransferUpdateDeleteErrorErrorMessage);
            }

                       
            await context.SaveChangesAsync();

            return transferToUpdate;
        }



        public async Task<PaginatedList<Transfer>> FilterByAsync(TransferQueryParameters transferQueryParameters, User loggedUser)
        {
            var result = await this.transferRepository.FilterByAsync(transferQueryParameters, loggedUser);

            if (result.Count == 0)
            {
                throw new EntityNotFoundException(Constants.ModifyTransferNoDataErrorMessage);
            }

            return await Task.FromResult(result);

        }

        public async Task<bool> ExecuteAsync(int transferId, User user)
        {
            Transfer transferToGet = await transferRepository.GetByIdAsync(transferId);

            if (!(await Common.IsUserAuthorizedAsync(transferToGet, user)))
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransferErrorMessage);
            }

            var transferToExecute = await this.transferRepository.GetByIdAsync(transferId);
            transferToExecute.IsConfirmed = true;
            transferToExecute.DateCreated = DateTime.Now;

            if (transferToExecute.TransferType == TransferDirection.Deposit)
            {
                this.accountRepository.IncreaseBalanceAsync(transferToExecute.AccountId, transferToExecute.Amount);

                this.cardRepository.DecreaseBalanceAsync(transferToExecute.CardId, transferToExecute.Amount);
            }

            if (transferToExecute.TransferType == TransferDirection.Withdrawal)
            {
                this.accountRepository.DecreaseBalanceAsync(transferToExecute.AccountId, transferToExecute.Amount);

                this.cardRepository.IncreaseBalanceAsync(transferToExecute.CardId, transferToExecute.Amount);
            }

            AddTransferToHistoryAsync(transferToExecute);

            return transferToExecute.IsConfirmed;

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
