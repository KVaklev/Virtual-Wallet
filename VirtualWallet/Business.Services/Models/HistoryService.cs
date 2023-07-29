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

       
        public GetHistoryDto GetById(int id, User user)
        {
            if (!user.IsAdmin || IsUserUnauthorized(id, user)!=true)
            {
                throw new UnauthorizedOperationException(Constants.ModifyUnauthorizeErrorMessage);
            }
            
            var history = historyRepository.GetById(id);
            var hidtoriDto = MapHistoryToDto(history);
            return hidtoriDto;
        }

        
        //todo - paginated
        public List<GetHistoryDto> FilterBy(HistoryQueryParameters filterParameters, User user)
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
            var resultDto = result.Select(r=>MapHistoryToDto(r)).ToList();
            return resultDto;
        }

        private GetHistoryDto MapHistoryToDto(History history)
        {
            var historyDto = new GetHistoryDto();
            historyDto.EventTime = history.EventTime.ToString();
            historyDto.NameOperation = history.NameOperation.ToString();

            if (history.TransactionId != null)
            {
                historyDto.From = history.Transaction.AccountSender.User.Username;
                historyDto.To = this.transactionRepository.GetById((int)history.TransactionId).AccountRecepient.User.Username;
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
