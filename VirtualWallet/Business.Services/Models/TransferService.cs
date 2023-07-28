using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using DataAccess.Repositories.Models;
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
            return this.transferRepository.GetAll(username);
        }

        public Transfer GetById(int id, User user)
        {
            if (!IsUserAuthorized(id, user.Id) || user.IsAdmin != true)
            {
                throw new UnauthorizedOperationException(Constants.ModifyUnauthorizeErrorMessage);
            }

            return this.transferRepository.GetById(id);
        }

        public Transfer Create(Transfer transfer, User user)
        {
            if (user.IsBlocked)
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransferErrorMessage);
            }

            if (this.accountRepository.CheckBalance(transfer.AccountId, transfer.Amount))
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransferAmountErrorMessage);
            }

            var createdTransfer = this.transferRepository.Create(transfer);

            return createdTransfer;
        }

        public bool Delete(int id, User user)
        {
            if (!IsUserAuthorized(id, user.Id))
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransferErrorMessage);
            }

            return this.transferRepository.Delete(id);
        }

        public Transfer Update(int id, Transfer transfer, User user)
        {
            if (!IsUserAuthorized(id, user.Id))
            {
                throw new UnauthenticatedOperationException(Constants.ModifyTransferErrorMessage);
            }

            return this.transferRepository.Update(id, transfer);
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

        public bool Execute(int transferId, User user)
        {
            if (!IsUserAuthorized(transferId, user.Id))
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransferErrorMessage);
            }

            var transferToExecute = this.transferRepository.GetById(transferId);
            transferToExecute.IsConfirmed = true;
            transferToExecute.DateCreated = DateTime.Now;

            if (transferToExecute.TransferType == TransferDirection.Deposit)
            {
                this.accountRepository.IncreaseBalance(transferToExecute.AccountId, transferToExecute.Amount);

                this.cardRepository.DecreaseBalance(transferToExecute.CardId, transferToExecute.Amount);
            }

            if (transferToExecute.TransferType == TransferDirection.Withdrawal)
            {
                this.accountRepository.DecreaseBalance(transferToExecute.AccountId, transferToExecute.Amount);

                this.cardRepository.IncreaseBalance(transferToExecute.CardId, transferToExecute.Amount);
            }

            AddTransferToHistory(transferToExecute);

            return transferToExecute.IsConfirmed;

        }
        public bool AddTransferToHistory(Transfer transfer)
        {
            var historyCount = this.context.History.Count();

            var history=this.historyRepository.CreateWithTransfer(transfer);
            int historyCountNewHistoryAdded = this.context.History.Count();

            if (historyCount + 1 == historyCountNewHistoryAdded)
            {
                return true;
            }
            else
            {
                return false;
            }


        }
        public bool IsUserAuthorized(int id, int userId)
        {
            bool IsUserAuthorized = true;

            Transfer transferToGet = this.transferRepository.GetById(id);

            if (transferToGet.Account.UserId != userId)
            {
                IsUserAuthorized = false;
            }

            return IsUserAuthorized;

        }

    }


}
