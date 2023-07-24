using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
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

        public TransferService(ITransferRepository transferRepository)
        {
            this.transferRepository = transferRepository;
        }

        public IQueryable<Transfer> GetAll()
        {
            return this.transferRepository.GetAll(); ;
        }

        public Transfer GetById(int id, User user)
        {
            if(!IsUserAuthorized(id, user.Id) || user.IsAdmin != true)
            {
                throw new UnauthorizedOperationException(Constants.ModifyTransactionErrorMessage);
            }

            return this.transferRepository.GetById(id);
        }

        public Transfer Create(Transfer transfer, User user)
        {
            throw new NotImplementedException();
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

        private bool IsUserAuthorized (int id, int userId)
        {
            bool isAuthorized = true;

            Transfer transferToUpdate = this.transferRepository.GetById(id);

            if(transferToUpdate.Account.User.Id != userId)
            {
                isAuthorized = false;
            }
            return isAuthorized;
        }
    }


}
