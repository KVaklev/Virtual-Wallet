﻿using Business.QueryParameters;
using DataAccess.Models.Models;

namespace DataAccess.Repositories.Contracts
{
    public interface ITransferRepository
    {
        IQueryable<Transfer> GetAll(string username);
        //paginated list

        Transfer GetById(int id);

        Transfer GetByUserId(int userId);

        Transfer Create(Transfer transfer);

        Transfer Update(int id, Transfer transfer);

        bool Delete(int id);

        PaginatedList<Transfer> FilterBy(TransferQueryParameters filterParameters, string username);








    }
}
