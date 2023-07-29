using Business.Exceptions;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Business.Services.Helpers;
using Business.QueryParameters;
using DataAccess.Repositories.Models;
using AutoMapper;
using Business.Dto;
using Business.DTOs;
using DataAccess.Repositories.Data;

namespace Business.Services.Models
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IAccountRepository accountRepository;
        private readonly IAccountService accountService;
        private readonly IMapper mapper;
        private readonly ApplicationContext context;

        public UserService(
            IUserRepository userRepository,
            IAccountRepository accountRepository,
            IAccountService accountService,
            IMapper mapper,
            ApplicationContext context
            )
        {
            this.userRepository = userRepository;
            this.accountRepository = accountRepository;
            this.accountService = accountService;
            this.mapper = mapper;
            this.context = context;
        }

        //public List<User> GetAll()
        //{
        //    return this.repository.GetAll();
        //}

        public async Task<List<User>> GetAllAsync()
        {
            return await this.repository.GetAllAsync();
        }
        //public List<User> FilterBy(UserQueryParameters filterParameters)
        //{
        //    return this.repository.FilterBy(filterParameters);
        //}

        public async Task<List<User>> FilterByAsync(UserQueryParameters filterParameters)
        {
            return await this.repository.FilterByAsync(filterParameters);
        }

        public User GetById(int id)
        {
            return this.repository.GetById(id);
        }
        public User GetByUsername(string username)
        {
            return this.repository.GetByUsername(username);
        }
        public User GetByEmail(string email)
        {
            return this.repository.GetByEmail(email);
        }
        public User GetByPhoneNumber(string pnoneNumber)
        {
            return this.repository.GetByPhoneNumber(pnoneNumber);
        }

        public async Task<GetUserDto> CreateAsync(CreateUserDto createUserDto)
        {
            User user = mapper.Map<User>(createUserDto);
            if (await UsernameExistsAsync(user.Username))
            {
                throw new DuplicateEntityException($"User with username '{user.Username}' already exists.");
            }

            if (EmailExists(user.Email))
            {
                throw new DuplicateEntityException($"User with email '{user.Email}' already exists.");
            }

            if (PhoneNumberExists(user.PhoneNumber))
            {
                throw new DuplicateEntityException($"User with phone number '{user.PhoneNumber}' already exists.");
            }
           
            User createdUser = await this.userRepository.CreateAsync(user);
            var createAccountDto = new CreateAccountDto()
            {
                Abbreviation = createUserDto.Abbreviation
            };
            Account createdAcount = await this.accountService.CreateAsync(createAccountDto, createdUser);
            createdUser.AccountId = createdAcount.Id;
            await context.SaveChangesAsync();
            GetUserDto getUserDto = mapper.Map<GetUserDto>(createdUser);

            return getUserDto;

        }
        public User Update(int id, User user, User loggedUser)
        {
            User userToUpdate = this.repository.GetById(id);

            if (!IsAuthorized(userToUpdate, loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }

            if (user.Email != userToUpdate.Email)
            {
                if (EmailExists(user.Email))
                {
                    throw new DuplicateEntityException($"User with email '{user.Email}' already exists.");
                }
            }

            if (user.PhoneNumber != userToUpdate.PhoneNumber)
            {
                if (PhoneNumberExists(user.PhoneNumber))
                {
                    throw new DuplicateEntityException($"User with phone number '{user.PhoneNumber}' already exists.");
                }
            }

            userToUpdate = this.repository.Update(id, loggedUser);
            return userToUpdate;
        }

        public bool Delete(int id, User loggedUser)
        {
            User userToDelete = this.repository.GetById(id);

            if (!IsAuthorized(userToDelete, loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            } 
            
             var accountToDelete = await this.accountRepository.GetByIdAsync((int)userToDelete.AccountId);
             await this.accountService.DeleteAsync(accountToDelete.Id, loggedUser);

            return this.repository.Delete(id);
        }
        public User Promote(int id, User loggedUser)
        {
            if (!IsAdmin(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }
            return this.repository.Promote(id);
        }
        public User BlockUser(int id, User loggedUser)
        {
            if (!IsAdmin(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }
            return this.repository.BlockUser(id);
        }
        public User UnblockUser(int id, User loggedUser)
        {
            if (!IsAdmin(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }
            return this.repository.UnblockUser(id);
        }
        public bool EmailExists(string email)
        {
            return this.repository.EmailExists(email);
        }
        public bool UsernameExists(string username)
        {
            return this.repository.UsernameExists(username);
        }
        public bool PhoneNumberExists(string phoneNumber)
        {
            return this.repository.PhoneNumberExists(phoneNumber);
        }
        public bool IsAuthorized(User user, User loggedUser)
        {
            bool isAuthorized = false;

            if (user.Id == loggedUser.Id || loggedUser.IsAdmin)
            {
                isAuthorized = true;
            }
            return isAuthorized;
        }
        public bool IsAdmin(User loggedUser)
        {
            if (!loggedUser.IsAdmin)
            {
                return false;
            }
            return true;
        }
    }
}
