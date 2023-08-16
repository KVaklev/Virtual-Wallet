using Business.QueryParameters;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using AutoMapper;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.Mappers;
using DataAccess.Models.Enums;

namespace Business.Services.Models
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IAccountRepository accountRepository;
        private readonly ICurrencyRepository currencyRepository;
        private readonly IExchangeRateService exchangeRateService;
        private readonly IAccountService accountService;
        private readonly IHistoryRepository historyRepository;
        private readonly IMapper mapper;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository,
            ICurrencyRepository currencyRepository,
            IExchangeRateService exchangeRateService,
            IAccountService accountService,
            IHistoryRepository historyRepository,
            IMapper mapper)

        {
            this.transactionRepository = transactionRepository;
            this.accountRepository = accountRepository;
            this.currencyRepository = currencyRepository;
            this.exchangeRateService = exchangeRateService;
            this.accountService = accountService;
            this.historyRepository= historyRepository;
            this.mapper = mapper;
        }
        public async Task<Response<GetTransactionDto>> GetByIdAsync(int id, User loggedUser)
        {
            var result = new Response<GetTransactionDto>();
            var transaction = await this.transactionRepository.GetByIdAsync(id);
            var checksResult = await TransactionChecker.ChecksGetByIdAsync(transaction, loggedUser);
            if (!checksResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = checksResult.Message;
                return result;
            }
            var transactionDto = this.mapper.Map<GetTransactionDto>(transaction);
            result.Data = transactionDto;

            return result;
        }

        public async Task<Response<PaginatedList<GetTransactionDto>>> FilterByAsync(
            TransactionQueryParameters filterParameters,
            User loggedUser)
        {
            var result = new Response<PaginatedList<GetTransactionDto>>();
            IQueryable<Transaction> transactions = this.transactionRepository.GetAll();

            transactions = await FilterByUsernameAsync(transactions, filterParameters.Username);
            transactions = await FilterByDirectionAsync(transactions, filterParameters.Direction);
            transactions = await FilterByFromDataAsync(transactions, filterParameters.FromDate);
            transactions = await FilterByToDataAsync(transactions, filterParameters.ToDate);
            transactions = await SortByAsync(transactions, filterParameters.SortBy);
            transactions = await GetLoogedUserTransactionsAsync(transactions, loggedUser);

            int totalPages = (transactions.Count() + filterParameters.PageSize - 1) / filterParameters.PageSize;
            transactions = await Common<Transaction>
                            .PaginateAsync(transactions, filterParameters.PageNumber, filterParameters.PageSize);
            if (!transactions.Any())
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }
            var transactionDtos = transactions
                            .Select(transaction => mapper.Map<GetTransactionDto>(transaction))
                            .ToList();
            result.Data = new PaginatedList<GetTransactionDto>(transactionDtos, totalPages, filterParameters.PageNumber);
            
            return result;
        }

        public async Task<Response<GetTransactionDto>> CreateOutTransactionAsync(
            CreateTransactionDto transactionDto, 
            User loggedUser)
        {
            var result = new Response<GetTransactionDto>();    
            var recipient = await accountRepository.GetByUsernameAsync(transactionDto.RecipientUsername);
            var currency = await currencyRepository.GetByCurrencyCodeAsync(transactionDto.CurrencyCode);
            var exchangeRate = await this.exchangeRateService
                               .GetExchangeRateAsync(transactionDto.CurrencyCode, loggedUser.Account.Currency.CurrencyCode);
            var checksResult = await TransactionChecker
                               .ChecksCreateOutTransactionAsync(transactionDto, loggedUser, recipient, currency, exchangeRate);
            if (!checksResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = checksResult.Message;
                return result;
            }
            var transaction = await TransactionsMapper.MapDtoТоTransactionAsync(transactionDto, loggedUser, recipient, currency, exchangeRate.Data);
            var newTransaction = await this.transactionRepository.CreateTransactionAsync(transaction);
            result.Data = this.mapper.Map<GetTransactionDto>(newTransaction);

            return result;
        }

        public async Task<Response<GetTransactionDto>> UpdateAsync(int id, User loggedUser, CreateTransactionDto transactionDto)
        {
            var result = new Response<GetTransactionDto>();
            var transactionToUpdate = await this.transactionRepository.GetByIdAsync(id);  
            var recipient = await accountRepository.GetByUsernameAsync(transactionDto.RecipientUsername);
            if (recipient==null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }
            var currency = await currencyRepository.GetByCurrencyCodeAsync(transactionDto.CurrencyCode);
            var exchangeRate = await this.exchangeRateService
                       .GetExchangeRateAsync(transactionDto.CurrencyCode, loggedUser.Account.Currency.CurrencyCode);
            var checksResult = await TransactionChecker
                       .ChecksUpdateAsync(transactionToUpdate, loggedUser, transactionDto, recipient, currency, exchangeRate);
            if (!checksResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = checksResult.Message;
                return result;
            }
            var updatedTransacion = await TransactionsMapper
                       .MapUpdateDtoToTransactionAsync(transactionToUpdate, transactionDto, recipient, currency, exchangeRate.Data);
            await this.transactionRepository.SaveChangesAsync();
            result.Data = this.mapper.Map<GetTransactionDto>(updatedTransacion);
           
            return result;
        }

        public async Task<Response<bool>> DeleteAsync(int id, User loggedUser)
        {
            var result = new Response<bool>();
            var transaction = await this.transactionRepository.GetByIdAsync(id);
            var checksResult = await TransactionChecker.ChecksDeleteAsync(transaction, loggedUser);
            if (!checksResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = checksResult.Message;
                return result;
            }
            result.Message = Constants.ModifyTransactionDeleteMessage;
            result.Data = await this.transactionRepository.DeleteAsync(transaction);
            return result;
        }

        public async Task<Response<bool>> ConfirmAsync(int transactionId, User loggedUser)
        {
            var result = new Response<bool>();
            var transactionOut = await this.transactionRepository.GetByIdAsync(transactionId);
            var checksResult = await TransactionChecker.ChecksConfirmAsync(transactionOut, loggedUser);
            if (!checksResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = checksResult.Message;
                return result;
            }
            var transactionInResult = await CreateInTransactionAsync(transactionOut);
            if (!transactionInResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = transactionInResult.Message;
                return result;
            }
            var updareResult =  await UpdateAccountsBalancesAsync(transactionOut, transactionInResult.Data);
            if (!updareResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = updareResult.Message;
                return result;
            }
            var addTransactionOutResult = await AddTransactionToHistoryAsync(transactionOut);
            if (!addTransactionOutResult)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NotSaved;
                return result;
            }
            var addTransactionInResult = await AddTransactionToHistoryAsync(transactionInResult.Data);
            if (!addTransactionInResult)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NotSaved;
                return result;
            }
            transactionOut.IsConfirmed = true;
            transactionOut.Date = DateTime.Now;
            await transactionRepository.SaveChangesAsync();

            result.Message = Constants.ModifyTransactionConfirmMessage;
            result.Data = transactionOut.IsConfirmed;

            return result;
        }

        private async Task<Response<Transaction>> CreateInTransactionAsync(Transaction transaction)
        {
            var result = new Response<Transaction>();

            var exchangeAmountResult = await this.exchangeRateService.ExchangeAsync(
                  transaction.Amount, 
                  transaction.Currency.CurrencyCode, 
                  transaction.AccountRecipient.Currency.CurrencyCode);
                if (!exchangeAmountResult.IsSuccessful)
                {
                    result.IsSuccessful = false;
                    result.Message = exchangeAmountResult.Message;
                    return result;
                }

            var exchangeRate = await this.exchangeRateService.GetExchangeRateAsync(
                transaction.Currency.CurrencyCode, 
                transaction.AccountRecipient.Currency.CurrencyCode);
            if (!exchangeRate.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoRecordsFound;
                return result;
            }

            var transactionIn = await TransactionsMapper
                            .MapOutToInTransactionAsync(transaction, exchangeAmountResult.Data, exchangeRate.Data);
            await this.transactionRepository.CreateTransactionAsync(transactionIn);
            result.Data = transactionIn;
            return result;
        }
        private async Task<Response<bool>> UpdateAccountsBalancesAsync(
            Transaction transactionOut, 
            Transaction transactionIn)
        {
            var result = new Response<bool>();
            var acountDecrResult = await this.accountService.DecreaseBalanceAsync(
                transactionOut.AccountSenderId, 
                (decimal)transactionOut.AmountExchange, 
                transactionOut.AccountSender.User);
            if (!acountDecrResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = acountDecrResult.Message;
                return result;
            }

           var acountIncrResult = await this.accountService.IncreaseBalanceAsync(
                transactionIn.AccountRecepientId, 
                transactionIn.ExchangeRate, 
                transactionIn.AccountRecipient.User);
            if (!acountIncrResult.IsSuccessful)
            {
                result.IsSuccessful = false;
                result.Message = acountIncrResult.Message;
                return result;
            }

            result.Data = true;
            return result;
        }

        private async Task<bool> AddTransactionToHistoryAsync(Transaction transaction)
        {

            int historyCount = await this.historyRepository.GetHistoryCountAsync();
            History history = await HistoryMapper.MapCreateWithTransactionAsync(transaction);
            await this.historyRepository.CreateAsync(history);
            int newHistoryCount = await this.historyRepository.GetHistoryCountAsync();

            if (newHistoryCount == historyCount + 1) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async Task<IQueryable<Transaction>> FilterByUsernameAsync(
            IQueryable<Transaction> result, 
            string? username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                return result.Where(transaction =>
                        (transaction.Direction == DirectionType.Out
                        && transaction.AccountRecipient.User.Username == username)
                        || (transaction.Direction == DirectionType.In
                        && transaction.AccountSender.User.Username == username));
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<Transaction>> FilterByFromDataAsync(
            IQueryable<Transaction> result, 
            string? fromData)
        {
            if (!string.IsNullOrEmpty(fromData))
            {
                DateTime date = DateTime.Parse(fromData);
                return result.Where(t => t.Date >= date);
            }
            return result;
        }
        private async Task<IQueryable<Transaction>> FilterByToDataAsync(
            IQueryable<Transaction> result, 
            string? toData)
        {
            if (!string.IsNullOrEmpty(toData))
            {
                DateTime date = DateTime.Parse(toData);

                return result.Where(t => t.Date <= date);
            }
            return result;
        }
        private async Task<IQueryable<Transaction>> FilterByDirectionAsync(
            IQueryable<Transaction> result, 
            string? direction)
        {
            if (!string.IsNullOrEmpty(direction))
            {
                if (Enum.TryParse<DirectionType>(direction, true, out var directionEnum))
                {
                    DirectionType directionType = directionEnum;
                    return await Task.FromResult(result.Where(t => t.Direction == directionType));
                }
            }
            return result;
        }
        private async Task<IQueryable<Transaction>> SortByAsync(IQueryable<Transaction> result, string sortCriteria)
        {
            if (Enum.TryParse<SortCriteria>(sortCriteria, true, out var sortEnum))
            {
                switch (sortEnum)
                {
                    case SortCriteria.Amount:
                        return await Task.FromResult(result.OrderBy(t => t.Amount));
                    case SortCriteria.Date:
                        return await Task.FromResult(result.OrderBy(t => t.Date));
                }
            }
            return result;
        }
        private async Task<IQueryable<Transaction>> GetLoogedUserTransactionsAsync(IQueryable<Transaction> userTransactions, User loggedUser)
        {
             
            if (!loggedUser.IsAdmin)
            {
                userTransactions = userTransactions
                    .Where(transaction =>
                        (transaction.Direction == DirectionType.Out 
                        && transaction.AccountSenderId == loggedUser.AccountId) 
                        || (transaction.Direction == DirectionType.In 
                        && transaction.AccountRecepientId == loggedUser.AccountId)
                    );
            }

            return userTransactions;
        }
    }
}
