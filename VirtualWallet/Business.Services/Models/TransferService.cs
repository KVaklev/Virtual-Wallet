using AutoMapper;
using Business.DTOs;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Exceptions;
using Business.Mappers;
using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Enums;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;


namespace Business.Services.Models
{
    public class TransferService : ITransferService
    {
        private readonly ITransferRepository transferRepository;
        private readonly ICardRepository cardRepository;
        private readonly IMapper mapper;
        private readonly ICurrencyRepository currencyRepository;
        private readonly IAccountService accountService;
        private readonly ICardService cardService;
        private readonly IExchangeRateService exchangeRateService;
        private readonly IHistoryRepository historyRepository;
        private readonly ISecurityService security;

        public TransferService(
            ITransferRepository transferRepository,            
            ICardRepository cardRepository,
            IHistoryRepository historyRepository,
            IMapper mapper,
            ICurrencyRepository currencyRepository,
            IAccountService accountService,
            ICardService cardService,
            IExchangeRateService exchangeRateService,
            ISecurityService security)
        {
            this.transferRepository = transferRepository;
            this.mapper = mapper;
            this.currencyRepository = currencyRepository;
            this.cardRepository = cardRepository;
            this.accountService = accountService;
            this.cardService = cardService;
            this.exchangeRateService = exchangeRateService;
            this.historyRepository = historyRepository;
            this.security = security;
        }

        public async Task<Response<PaginatedList<GetTransferDto>>> FilterByAsync(TransferQueryParameters filterParameters, User loggedUser)
        {
            var result = new Response<PaginatedList<GetTransferDto>>();
            var transfersResult = this.GetAll(loggedUser);

            if(transfersResult.Data == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;

            }
            IQueryable<Transfer> transfers = transfersResult.Data;

            transfers = await FilterByUsernameAsync(transfers, filterParameters.Username);
            transfers = await FilterByFromDateAsync(transfers, filterParameters.FromDate);
            transfers = await FilterByToDateAsync(transfers, filterParameters.ToDate);
            transfers = await FilterByTransferTypeAsync(transfers, filterParameters.TransferType);
            transfers = await SortByAsync(transfers, filterParameters.SortBy);

            int totalPages = (transfers.Count() + filterParameters.PageSize - 1) / filterParameters.PageSize;
            transfers = await Common<Transfer>.PaginateAsync(transfers, filterParameters.PageNumber, filterParameters.PageSize);

            if (!transfers.Any())
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyNoRecordsFound;
                return result;
            }

            List<GetTransferDto> transferDtos = transfers
                .Select(transfer => mapper.Map<GetTransferDto>(transfer)).ToList();

            result.Data = new PaginatedList<GetTransferDto>(transferDtos, totalPages, filterParameters.PageNumber);
            return result;
        }

        public async Task<Response<GetTransferDto>> GetByIdAsync(int id, User user)
        {
            var result = new Response<GetTransferDto>();

            Transfer transferToGet = await transferRepository.GetByIdAsync(id);

            if (transferToGet == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyNoRecordsFound;
                return result;
            }

            if (!await this.security.IsUserAuthorizedAsync(transferToGet, user) && user.IsAdmin)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransferGetByIdErrorMessage;
                return result;
            }

            var transferDto = this.mapper.Map<GetTransferDto>(transferToGet);
            result.Data = transferDto;

            return result;
        }

        public async Task<Response<GetTransferDto>> CreateAsync(CreateTransferDto transferDto, User user)
        {
            var result = new Response<GetTransferDto>();

            if (user.IsBlocked)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransferErrorMessage;
                return result;
            }

            var card = this.cardRepository.GetByAccountId((int)user.AccountId).FirstOrDefault(x => x.CardNumber == transferDto.CardNumber);

            if (card == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }

            var currency = await this.currencyRepository.GetByCurrencyCodeAsync(transferDto.CurrencyCode);

            if (currency == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }
            var transferType = Enum.Parse<TransferDirection>(transferDto.TransferType, true);

            var transfer = await TransfersMapper.MapCreateDtoToTransferAsync(transferDto, user, card, currency, transferType);

            if (transfer.TransferType == TransferDirection.Deposit)
            {
                if (!await Common.HasEnoughCardBalanceAsync(transfer.Card, transfer.Amount))
                {
                    result.IsSuccessful = false;
                    result.Message = Constants.ModifyAccountBalancetErrorMessage;

                    return result;
                }
            }
            else
            {
                if (!await Common.HasEnoughBalanceAsync(transfer.Account, transfer.Amount))
                {
                    result.IsSuccessful = false;
                    result.Message = Constants.ModifyAccountBalancetErrorMessage;

                    return result;
                }
            }

            transfer.DateCreated = DateTime.UtcNow;
            transfer.IsConfirmed = false;
            transfer.IsCancelled = false;

            var newTransfer = await this.transferRepository.CreateAsync(transfer);

            result.Data = this.mapper.Map<GetTransferDto>(newTransfer);

            return result;
        }

        public async Task<Response<bool>> DeleteAsync(int id, User user)
        {
            var result = new Response<bool>();

            Transfer transferToDelete = await transferRepository.GetByIdAsync(id);

            if (transferToDelete == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyNoRecordsFound;
                return result;
            }

            if (!await this.security.IsUserAuthorizedAsync(transferToDelete, user))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransferErrorMessage;
                return result;

            }

            if (transferToDelete.IsCancelled || transferToDelete.IsConfirmed)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransferUpdateDeleteErrorErrorMessage;
                return result;

            }
            result.Message = Constants.ModifyTransferErrorMessage;
            result.Data = await this.transferRepository.DeleteAsync(id);
            return result;
        }

        public async Task<Response<GetTransferDto>> UpdateAsync(int id, UpdateTransferDto transferDto, User user)
        {
            var result = new Response<GetTransferDto>();
            Transfer transfer = await transferRepository.GetByIdAsync(id);

            if (transfer == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyNoRecordsFound;
                return result;
            }

            if (!await this.security.IsUserAuthorizedAsync(transfer, user))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransferErrorMessage;
                return result;
            }

            if (!await Common.HasEnoughBalanceAsync(transfer.Account, transfer.Account.Balance))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyAccountBalancetErrorMessage;
                return result;
            }

            if (transfer.IsConfirmed || transfer.IsCancelled)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransferUpdateDeleteErrorErrorMessage;
                return result;
            }

            var card = this.cardRepository.GetByAccountId((int)user.AccountId).FirstOrDefault(x => x.CardNumber == transferDto.CardNumber);
            var currency = await this.currencyRepository.GetByCurrencyCodeAsync(transferDto.CurrencyCode);


            var newTransfer = await TransfersMapper.MapUpdateDtoToTransferAsync(transfer, transferDto, card, currency);

            newTransfer.DateCreated = DateTime.UtcNow;

            result.Data = this.mapper.Map<GetTransferDto>(newTransfer);

            await this.transferRepository.SaveChangesAsync();

            return result;
        }

        public async Task<Response<bool>> ConfirmAsync(int transferId, User user)
        {
            var result = new Response<bool>();

            Transfer transferToExecute = await transferRepository.GetByIdAsync(transferId);

            if (transferToExecute == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyNoRecordsFound;
                return result;
            }

            if (!await this.security.IsUserAuthorizedAsync(transferToExecute, user))
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransferErrorMessage;
                return result;
            }

            if (transferToExecute.IsConfirmed || transferToExecute.IsCancelled)
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyTransferUpdateDeleteErrorErrorMessage;
                return result;
            }


            transferToExecute.IsConfirmed = true;
            transferToExecute.DateCreated = DateTime.UtcNow;

            await UpdateAccountsBalanceAsync(transferToExecute, user);

            await AddTransferToHistoryAsync(transferToExecute);

            await transferRepository.SaveChangesAsync();

            result.Message = Constants.ModifyExecutedTransfer;
            result.Data = transferToExecute.IsConfirmed;
            return result;

        }

        private Response<IQueryable<Transfer>> GetAll(User loggedUser)
        {
            var result = new Response<IQueryable<Transfer>>();
            var transfers = this.transferRepository.GetAll(loggedUser);

            if (transfers.Any())
            {
                if (!loggedUser.IsAdmin)
                {
                    transfers = transfers
                         .Where(a => a.Account.User.Username == loggedUser.Username)
                         .AsQueryable();
                }
                result.Data = transfers;
                return result;
            }
            else
            {
                result.IsSuccessful = false;
                result.Message = Constants.ModifyNoRecordsFound;
                return result;
            }
        }

        private async Task<Response<decimal>> GetCorrectAmountAsync(string transferCurrencyCode, string accountCurrencyCode, decimal amount)
        {
            var result = new Response<decimal>();

            result.Data = amount;

            if (transferCurrencyCode != accountCurrencyCode)
            {
                var amountResult = await this.exchangeRateService.ExchangeAsync(amount, transferCurrencyCode, accountCurrencyCode);

                if (!amountResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.Message = amountResult.Message;
                    return result;
                }

                result.Data = amountResult.Data;

            }

            return result;
        }

        private async Task<Response<bool>> UpdateAccountsBalanceAsync(Transfer transfer, User user)
        {
            var accountAmount = await GetCorrectAmountAsync(transfer.Currency.CurrencyCode, transfer.Account.Currency.CurrencyCode, transfer.Amount);

            var result = new Response<bool>();

            if (!accountAmount.IsSuccessful)
            {

                result.IsSuccessful = false;
                result.Message = accountAmount.Message;
                return result;

            }

            var cardAmount = await GetCorrectAmountAsync(transfer.Currency.CurrencyCode, transfer.Card.Currency.CurrencyCode, transfer.Amount);

            if (!cardAmount.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = cardAmount.Message;
                return result;
            }

            if (transfer.TransferType == TransferDirection.Deposit)
            {

                await this.accountService.IncreaseBalanceAsync(transfer.AccountId, accountAmount.Data, user);

                await this.cardService.DecreaseBalanceAsync(transfer.CardId, cardAmount.Data, user);
            }

            if (transfer.TransferType == TransferDirection.Withdrawal)
            {
                await this.accountService.DecreaseBalanceAsync(transfer.AccountId, accountAmount.Data, user);

                await this.cardService.IncreaseBalanceAsync(transfer.CardId, cardAmount.Data, user);
            }

            result.Data = true;

            return result;
        }

        private async Task<bool> AddTransferToHistoryAsync(Transfer transfer)
        {

            var historyCount = await this.historyRepository.GetHistoryCountAsync();

            var history = await HistoryMapper.MapCreateWithTransferAsync(transfer);

            await this.historyRepository.CreateAsync(history);

            int historyCountNewHistoryAdded = await this.historyRepository.GetHistoryCountAsync();

            if (historyCount + 1 == historyCountNewHistoryAdded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<IQueryable<Transfer>> FilterByUsernameAsync(IQueryable<Transfer> transfers, string? username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                transfers = transfers.Where(t => t.Account.User.Username == username);
            }

            return await Task.FromResult(transfers);
        }

        private async Task<IQueryable<Transfer>> FilterByFromDateAsync(IQueryable<Transfer> transfers, string? fromDate)
        {
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime date = DateTime.Parse(fromDate);

                transfers = transfers.Where(a => a.DateCreated >= date);
            }

            return await Task.FromResult(transfers);
        }

        private async Task<IQueryable<Transfer>> FilterByToDateAsync(IQueryable<Transfer> transfers, string? toDate)
        {
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime date = DateTime.Parse(toDate);

                transfers = transfers.Where(a => a.DateCreated <= date);
            }

            return await Task.FromResult(transfers);
        }

        private async Task<IQueryable<Transfer>> FilterByTransferTypeAsync(IQueryable<Transfer> transfers, string? transfer)
        {
            if (!string.IsNullOrEmpty(transfer))
            {
                if (TryParseTransferTypeParameter(transfer, out TransferDirection? transferType))
                {
                    if (transferType.HasValue)
                    {
                        return await Task.FromResult(transfers.Where(t => t.TransferType == transferType.Value));
                    }
                    else
                    {
                        return await Task.FromResult(transfers);
                    }
                }
            }

            return await Task.FromResult(transfers);
        }

        private bool TryParseTransferTypeParameter(string value, out TransferDirection? result)
        {
            if (Enum.TryParse(value, true, out TransferDirection parsedResult))
            {
                result = parsedResult;
                return true;
            }

            result = null;
            return false;
        }

        private static async Task<IQueryable<Transfer>> SortByAsync(IQueryable<Transfer> transfers, string? sortCriteria)
        {
            if (Enum.TryParse<SortCriteria>(sortCriteria, true, out var sortEnum))
            {
                switch (sortEnum)
                {
                    case SortCriteria.Amount:
                        return await Task.FromResult(transfers.OrderBy(t => t.Amount));
                    case SortCriteria.Date:
                        return await Task.FromResult(transfers.OrderBy(t => t.DateCreated));

                    default:
                        return await Task.FromResult(transfers);
                }
            }
            else
            {
                return await Task.FromResult(transfers);

            }
        }
    }
}
