using Business.Exceptions;
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

        public TransferService(ITransferRepository transferRepository, IHistoryRepository historyRepository, IAccountRepository accountRepository, IUserRepository userRepository, ICardRepository cardRepository, ApplicationContext context)
        {
            this.transferRepository = transferRepository;
            this.historyRepository = historyRepository;
            this.accountRepository = accountRepository;
            this.userRepository = userRepository;
            this.context = context;
        }

        public IQueryable<Transfer> GetAll(string username)
        {
            return transferRepository.GetAll(username);
        }

        public async Task <Transfer> GetByIdAsync(int id, User user)
        {
            if (!(await IsUserAuthorizedAsync(id, user.Id) || user.IsAdmin != true))
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransferGetByIdErrorMessage);
            }

            return await this.transferRepository.GetByIdAsync(id);
        }

        public async Task <Transfer> CreateAsync(Transfer transfer, User user)
        {
            if (user.IsBlocked)
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransferErrorMessage);
            }


            if (!await this.accountRepository.HasEnoughBalanceAsync(transfer.AccountId, transfer.Amount))
            {
                throw new UnauthorizedOperationException(Constants.ModifyAccountBalancetErrorMessage);
            }

            var createdTransfer = await this.transferRepository.CreateAsync(transfer);

            return createdTransfer;
        }

        public async Task <bool> DeleteAsync(int id, User user)
        {
            if (!await IsUserAuthorizedAsync(id, user.Id))
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransferErrorMessage);
            }

            return await this.transferRepository.DeleteAsync(id);
        }

        public async Task <Transfer> UpdateAsync(int id, Transfer transfer, User user)
        {
            if (!(await IsUserAuthorizedAsync(id, user.Id)))
            {
                throw new UnauthenticatedOperationException(Constants.ModifyTransferErrorMessage);
            }

            return await this.transferRepository.UpdateAsync(id, transfer);
        }

        public PaginatedList<Transfer> FilterBy(TransferQueryParameters transferQueryParameters, User user)
        {
            var result = this.transferRepository.FilterBy(transferQueryParameters, user.Username);

            if (result.Count == 0)
            {
                throw new EntityNotFoundException(Constants.ModifyTransferNoDataErrorMessage);
            }

            return result;
        }

        public async Task<bool> ExecuteAsync(int transferId, User user)
        {
            if (!(await IsUserAuthorizedAsync(transferId, user.Id)))
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransferErrorMessage);
            }

            var transferToExecute =  await this.transferRepository.GetByIdAsync(transferId);
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

            var history= await this.historyRepository.CreateWithTransferAsync(transfer);
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
        public async Task <bool> IsUserAuthorizedAsync(int id, int userId)
        {
            bool IsUserAuthorized = true;

            Transfer transferToGet = await this.transferRepository.GetByIdAsync(id);

            if (transferToGet.Account.UserId != userId)
            {
                IsUserAuthorized = false;
            }

            return IsUserAuthorized;

        }

    }


}
