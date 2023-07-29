using AutoMapper;
using Business.DTOs;
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
        ITransactionRepository transactionRepository;
        private readonly IMapper mapper;

        public HistoryService(
            IHistoryRepository historyRepository,
            ITransactionRepository transactionRepository,
            IMapper mapper
            )
        {
            this.historyRepository = historyRepository;
            this.transactionRepository = transactionRepository;
            this.mapper = mapper;
        }
        public IQueryable<History> GetAll(User loggedUser)
        {
            return this.historyRepository.GetAll(loggedUser);
        }

        public async Task<GetHistoryDto> GetByIdAsync(int id, User loggedUser)
        {
            if (!await Common.IsAdminAsync(loggedUser) || !await IsHistoryOwnerAsync(id, loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyHistoryErrorMessage);
            }
            
            var history = await historyRepository.GetByIdAsync(id);
            var historyDto = MapHistoryToDtoAsync(history);
            return historyDto;
        }

        public async Task<List<GetHistoryDto>> FilterByAsync(HistoryQueryParameters filterParameters, User loggedUser)
        {
            if (filterParameters.Username != null && !await Common.IsAdminAsync(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyHistoryErrorMessage);
            }
            
            var result = await this.historyRepository.FilterByAsync(filterParameters, loggedUser);

            var resultDto = result
                             .Select(result => MapHistoryToDtoAsync(result)) 
                             .ToList();

            return resultDto; //PaginatedList<GetHistoryDto>
           
        }

        private GetHistoryDto MapHistoryToDtoAsync(History history)
        {
            var historyDto = new GetHistoryDto();
            historyDto.EventTime = history.EventTime.ToString();
            historyDto.NameOperation = history.NameOperation.ToString();

            if (history.TransactionId != null)
            {
                historyDto.From = history.Transaction.AccountSender.User.Username;
                historyDto.To = history.Transaction.AccountRecepient.User.Username;
                historyDto.Amount = history.Transaction.Amount;
                historyDto.Аbbreviation = history.Transaction.Currency.Abbreviation;
                historyDto.Direction = history.Transaction.Direction.ToString();
            }
            else
            {
                historyDto.Amount = history.Transfer.Amount;
                historyDto.Аbbreviation = history.Transfer.Currency.Abbreviation;
                historyDto.Direction = history.Transfer.TransferType.ToString();

                if (history.Transfer.TransferType == TransferDirection.Deposit)
                {
                    historyDto.From = history.Transfer.Card.CardNumber;
                    historyDto.To = history.Transfer.Account.User.Username;
                }
                else
                {
                    historyDto.From = history.Transfer.Account.User.Username;
                    historyDto.To = history.Transfer.Card.CardNumber;
                }
            }
            return historyDto;
        }

        private async Task<bool> IsHistoryOwnerAsync(int id, User user)
        {
            bool isHistoryOwner = true;

            History history = await this.historyRepository.GetByIdAsync(id);

            if (history.AccountId!=user.AccountId)
            {
                isHistoryOwner = false;
            }
            return isHistoryOwner;
        }

    }
}
