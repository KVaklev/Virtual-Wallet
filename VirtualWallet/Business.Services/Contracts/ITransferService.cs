using Business.QueryParameters;
using DataAccess.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Contracts
{
    public interface ITransferService
    {
        IQueryable<Transfer> GetAll(string username);
        PaginatedList<Transfer> FilterBy(TransferQueryParameters transferQueryParameters, User user);
        Transfer GetById(int id, User user);
        Transfer Create(Transfer transfer, User user);
        Transfer Update(int id, Transfer transfer, User user);
        bool Delete(int id, User user);
        bool Execute(int transferId, User user);


    }
}
