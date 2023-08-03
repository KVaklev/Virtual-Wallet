using AutoMapper;
using Business.DTOs;
using Business.DTOs.Responses;
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

        public async Task<Response<GetHistoryDto>> GetByIdAsync(int id, User loggedUser)
        {
            var result = new Response<GetHistoryDto>();
            if (!loggedUser.IsAdmin || !await IsHistoryOwnerAsync(id, loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }
            
            var history = await historyRepository.GetByIdAsync(id);
            var historyDto = MapHistoryToDtoAsync(history);
            result.Data = historyDto;
            return result;
        }

        public async Task<Response<IQueryable<GetHistoryDto>>> FilterByAsync(
            HistoryQueryParameters filterParameters, 
            User loggedUser
            )
        {
            var result = new Response<IQueryable<GetHistoryDto>>();
            if (filterParameters.Username != null && !loggedUser.IsAdmin)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAuthorizedErrorMessage;
                return result;
            }
            
            var historyRecords = await this.historyRepository.FilterByAsync(filterParameters, loggedUser);
            var resultDto = historyRecords
                             .Select(history => MapHistoryToDtoAsync(history)) 
                             .AsQueryable();
            result.Data = resultDto;
            return result; // todo - PaginatedList<GetHistoryDto>
           
        }

        private GetHistoryDto MapHistoryToDtoAsync(History history)
        {
            var historyDto = new GetHistoryDto();
            historyDto.EventTime = history.EventTime.ToString();
            historyDto.NameOperation = history.NameOperation.ToString();

            if (history.TransactionId != null)
            {
                historyDto.From = history.Transaction.AccountSender.User.Username;
                historyDto.To = history.Transaction.AccountRecipient.User.Username;
                historyDto.Amount = history.Transaction.Amount;
                historyDto.CurrencyCode = history.Transaction.Currency.CurrencyCode;
                historyDto.Direction = history.Transaction.Direction.ToString();
            }
            else
            {
                historyDto.Amount = history.Transfer.Amount;
                historyDto.CurrencyCode = history.Transfer.Currency.CurrencyCode;
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
