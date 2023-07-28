using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;

namespace Business.Services.Models
{
    public class HistoryService : IHistoryService
    {
        private readonly IHistoryRepository historyRepository;

        public HistoryService(IHistoryRepository historyRepository)
        {
            this.historyRepository = historyRepository;
        }

       
        public History GetById(int id, User user)
        {
            if (!user.IsAdmin || IsUserUnauthorized(id, user)!=true)
            {
                throw new UnauthorizedOperationException(Constants.ModifyUnauthorizeErrorMessage);
            }
            return historyRepository.GetById(id);
        }

        public IQueryable<History> GetAll(User user)
        { 
              return historyRepository.GetAll(user);
        }

        public PaginatedList<History> FilterBy(HistoryQueryParameters filterParameters, User user)
        {
            if (filterParameters.Username!=null & !user.IsAdmin)
            {
                throw new UnauthorizedOperationException(Constants.ModifyUnauthorizeErrorMessage);
            }
            var result = this.historyRepository.FilterBy(filterParameters, user);
            if (result.Count == 0)
            {
                throw new EntityNotFoundException(Constants.ModifyTransactionNoDataErrorMessage);
            }
            return result;
        }

        private bool IsUserUnauthorized(int id, User user)
        {
            bool isUserUnauthorized = true;

            History history = this.historyRepository.GetById(id);

            if (history.AccountId!=user.AccountId)
            {
                isUserUnauthorized = false;
            }
            return isUserUnauthorized;
        }
    }
}
