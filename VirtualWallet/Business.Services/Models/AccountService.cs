using AutoMapper;
using Business.DTOs;
using Business.DTOs.Responses;
using Business.Mappers;
using Business.Services.Additional;
using Business.Services.Contracts;
using Business.Services.Helpers;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using static Business.Services.Helpers.Constants;

namespace Business.Services.Models
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository accountRepository;
        private readonly IUserRepository userRepository;
        private readonly ICardService cardService;
        private readonly ICurrencyService currencyService;
        private readonly IMapper mapper;
        private readonly ICardRepository cardRepository;

        public AccountService(
            IAccountRepository accountRepository,  
            IUserRepository userRepository,
            ICardService cardService,
            ICurrencyService currencyService,
            IMapper mapper,
            ICardRepository cardRepository
          )
        {
            this.accountRepository = accountRepository;
            this.userRepository = userRepository;
            this.cardService = cardService;
            this.currencyService = currencyService;
            this.mapper = mapper;
            this.cardRepository = cardRepository;           
        }
        public Response<IQueryable<GetAccountDto>> GetAll()
        {
            var result = new Response<IQueryable<GetAccountDto>>();
            var accounts = accountRepository.GetAll();

            if (accounts != null && accounts.Any())
            {
                result.IsSuccessful = true;
                result.Data = (IQueryable<GetAccountDto>)accounts.AsQueryable();
            }
            else
            {
                result.IsSuccessful = false;
                result.Message = NoAccountsErrorMessage;
            }

            return result;
        }

        public async Task<Response<GetAccountDto>> GetByIdAsync(int id, User user)
        {
            var result = new Response<GetAccountDto>();

            var account = await this.accountRepository.GetByIdAsync(id);
            if (account==null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoAccountsErrorMessage;
                return result;
            }
            
            if (!await Security.IsUserAuthorized(id, user))
            {
                result.IsSuccessful = false;
                result.Message = ModifyAccountErrorMessage;
                return result;
            }
            var accountDto = this.mapper.Map<GetAccountDto>(account);
            result.Data = accountDto;

            return result;
        }

        public async Task<Response<GetAccountDto>> GetByUsernameAsync(int id, User user)
        {
            var result = new Response<GetAccountDto>();
            
            var account = await accountRepository.GetByUsernameAsync(user.Username);
            if (account == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoUsersErrorMessage;
                return result;
            }

            if (!await Security.IsUserAuthorized(id, user))
            {
                result.IsSuccessful = false;
                result.Message = ModifyAccountErrorMessage;
                return result;
            }

            var accountDto = this.mapper.Map<GetAccountDto>(account);
            result.Data = accountDto;

            return result;
        }

        public async Task<Response<GetAccountDto>> CreateAsync(string currencyCode, User user)
        {
            var result = new Response<GetAccountDto>();

            var accountToCreate = new Account();
            var currency = await currencyService.GetByCurrencyCodeAsync(currencyCode);
            if (currency == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.CurrencyNotFoundErrorMessage;
                return result;
            }
            accountToCreate = await AccountsMapper.MapCreateDtoToAccountAsync(accountToCreate, currency, user);
            
            user.AccountId = user.Id;
            var createdAccount = await this.accountRepository.CreateAsync(accountToCreate);
            var accountDto = this.mapper.Map<GetAccountDto>(createdAccount);
            
            result.Data = accountDto;

            return result;
        }
        public async Task<Response<bool>> DeleteAsync(int id, User loggedUser)
        {
            var result = new Response<bool>();

            var accountToDelete = await this.accountRepository.GetByIdAsync(id);
            if (accountToDelete == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoAccountsErrorMessage;
                return result;
            }


            if (!await Security.IsAdminAsync(loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = ModifyUserErrorMessage;
                return result;
            }

            if (accountToDelete.Cards.Count != 0)
            {
                foreach (var card in accountToDelete.Cards)
                {
                    await this.cardService.DeleteAsync(card.Id, loggedUser);
                }
            }          

            result.Data = await this.accountRepository.DeleteAsync(id);

            return result;
        }
        public async Task<Response<bool>> AddCardAsync(int id, Card card, User user)
        {
            var result = new Response<bool>();

            if (!await Security.IsUserAuthorized(id, user))
            {
                result.IsSuccessful = false;
                result.Message = ModifyAccountCardErrorMessage;
                return result;
            }

            if (!await this.cardRepository.CardNumberExistsAsync(card.CardNumber))
            {
                result.IsSuccessful = true;
                result.Data = await this.accountRepository.AddCardAsync(id, card);
                result.Error = new Error(PropertyName.CardNumber);
                return result;
            }

            return result;
        }
        public async Task<Response<bool>> RemoveCardAsync(int id, Card card, User user)
        {
            var result = new Response<bool>();

            if (!await Security.IsUserAuthorized(id, user))
            {
                result.IsSuccessful = false;
                result.Message = ModifyAccountCardErrorMessage;
                return result;
            }
            if (!await this.cardRepository.CardNumberExistsAsync(card.CardNumber))
            {
                result.IsSuccessful = true;
                result.Data = await this.accountRepository.RemoveCardAsync(id, card);
                result.Error = new Error(PropertyName.CardNumber);
                return result;
            }

            return result;
        }
        public async Task<Response<string>> CreateApiTokenAsync(User loggedUser)
        {
            var result = new Response<string>();

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("This is my secret testing key"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                    issuer: "VirtualWallet",
                    audience: "Where is that audience",
                    claims: new[] {
                new Claim("LoggedUserId", loggedUser.Id.ToString()),
                new Claim("Username", loggedUser.Username),
                new Claim("IsAdmin", loggedUser.IsAdmin.ToString()),
                new Claim("UsersAccountId", loggedUser.Account.Id.ToString()),//null check
                new Claim(JwtRegisteredClaimNames.Email, loggedUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        },
            expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: signinCredentials
                );
            
            result.Data = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
           
            //todo - why
            if (!result.IsSuccessful)
            {
                result.Message = GenerateTokenErrorMessage;
                return result;
            }

            return await Task.FromResult(result);
        }
        public async Task<Response<string>> GenerateTokenAsync(int id)
        {
            var result = new Response<string>();

            var user = await this.userRepository.GetByIdAsync(id);
            if (user == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoUsersErrorMessage;
                return result;
            }

            result.Data = (DateTime.Now.ToString() + user.Username).ComputeSha256Hash();
           
            return result;
        }
        public async Task<Response<bool>> ConfirmRegistrationAsync(int id, string token)
        {
            var result = new Response<bool>();

            result.Data = await this.accountRepository.ConfirmRegistrationAsync(id, token);
            result.Message = ConfirmedRegistrationMessage;
            return result;
        }

        public async Task <Response<Account>> IncreaseBalanceAsync(int id, decimal amount, User loggedUser)
        {
            var result = new Response<Account>();
            Account accountToDepositTo = await this.accountRepository.GetByIdAsync(id);
            if (accountToDepositTo == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoAccountsErrorMessage;
                return result;
            }
            accountToDepositTo.Balance += amount;
            await this.accountRepository.SaveChangesAsync();
            result.Data = accountToDepositTo;
            return result;
            
        }

        public async Task<Response<Account>> DecreaseBalanceAsync(int id, decimal amount, User loggedUser)
        {
            var result = new Response<Account>();
            Account accountToWithdrawFrom = await this.accountRepository.GetByIdAsync(id);
            if (accountToWithdrawFrom == null)
            {
                result.IsSuccessful = false;
                result.Message = Constants.NoAccountsErrorMessage;
                return result;
            }
            accountToWithdrawFrom.Balance -= amount;
            await this.accountRepository.SaveChangesAsync();
            result.Data = accountToWithdrawFrom;
            return result;
        }
    }
}
