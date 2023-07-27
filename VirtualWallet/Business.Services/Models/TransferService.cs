using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
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
        private readonly ApplicationContext context;

        public TransferService(ITransferRepository transferRepository, IHistoryRepository historyRepository, IAccountRepository accountRepository, IUserRepository userRepository, ApplicationContext context)
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
                throw new UnauthorizedOperationException(Constants.ModifyTransactionErrorMessage);
            }

            return this.transferRepository.GetById(id);
        }

        public Transfer Create(Transfer transfer, User user)
        {
            if (user.IsBlocked)
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransferErrorMessage);
            }

            var createdTransfer = this.transferRepository.Create(transfer);

            return createdTransfer;
        }

        public void Delete(int id, User user)
        {
            throw new NotImplementedException();
        }

        public bool Execute(int transferId)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<Transfer> FilterBy(TransferQueryParameters transferQueryParameters)
        {
            throw new NotImplementedException();
        }


        public Transfer Update(int id, Transfer transfer, User user)
        {
            throw new NotImplementedException();
        }

        private bool IsUserAuthorized(int id, int userId)
        {
            bool isAuthorized = true;

            Transfer transferToUpdate = this.transferRepository.GetById(id);

            if (transferToUpdate.Account.User.Id != userId)
            {
                isAuthorized = false;
            }
            return isAuthorized;
        }

        public bool IsUserAuthorized(int id, User user)
        {
            bool IsUserAuthorized = false;

            Transfer transferToGet = this.transferRepository.GetById(id);

            if (transferToGet.Account.UserId == user.Id || user.IsAdmin)
            {
                IsUserAuthorized = true;
            }

            return IsUserAuthorized;

        }
    }


}
