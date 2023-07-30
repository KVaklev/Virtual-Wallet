﻿using Business.Exceptions;
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

        public UserService(
            IUserRepository userRepository,
            IAccountRepository accountRepository,
            IAccountService accountService,
            IMapper mapper
            )
        {
            this.userRepository = userRepository;
            this.accountRepository = accountRepository;
            this.accountService = accountService;
            this.mapper = mapper;
        }

        public IQueryable<User> GetAll()
        {
            return this.userRepository.GetAll();
        }

        public async Task<PaginatedList<User>> FilterByAsync(UserQueryParameters filterParameters)
        {
            return await this.userRepository.FilterByAsync(filterParameters);
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await this.userRepository.GetByIdAsync(id);
        }
        public async Task<User> GetByUsernameAsync(string username)
        {
            return await this.userRepository.GetByUsernameAsync(username);
        } 
        public async Task<User> GetByEmailAsync(string email)
        {
            return await this.userRepository.GetByEmailAsync(email);
        }
        public async Task<User> GetByPhoneNumberAsync(string pnoneNumber)
        {
            return await this.userRepository.GetByPhoneNumberAsync(pnoneNumber);
        }

        public async Task<GetUserDto> CreateAsync(CreateUserDto createUserDto)
        {
            User user = mapper.Map<User>(createUserDto);
            
            if (await UsernameExistsAsync(user.Username))
            {
                throw new DuplicateEntityException($"User with username '{user.Username}' already exists.");
            }

            if (await EmailExistsAsync(user.Email))
            {
                throw new DuplicateEntityException($"User with email '{user.Email}' already exists.");
            }

            if (await PhoneNumberExistsAsync(user.PhoneNumber))
            {
                throw new DuplicateEntityException($"User with phone number '{user.PhoneNumber}' already exists.");
            }
           
            User createdUser = await this.userRepository.CreateAsync(user);
            await this.accountService.CreateAsync(createUserDto.Abbreviation, createdUser);
 
            GetUserDto getUserDto = mapper.Map<GetUserDto>(createdUser);

            return getUserDto;

        }
        public async Task<User> UpdateAsync(int id, User user, User loggedUser)
        {
            User userToUpdate = await this.userRepository.GetByIdAsync(id);

            if (!await Common.IsAuthorizedAsync(userToUpdate, loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }

            if (user.Email != userToUpdate.Email)
            {
                if (await EmailExistsAsync(user.Email))
                {
                    throw new DuplicateEntityException($"User with email '{user.Email}' already exists.");
                }
            }

            if (user.PhoneNumber != userToUpdate.PhoneNumber)
            {
                if (await PhoneNumberExistsAsync(user.PhoneNumber))
                {
                    throw new DuplicateEntityException($"User with phone number '{user.PhoneNumber}' already exists.");
                }
            }

            userToUpdate = await this.userRepository.UpdateAsync(id, loggedUser);
            return userToUpdate;
        }

        public async Task<bool> DeleteAsync(int id, User loggedUser)
        {
            User userToDelete = await this.userRepository.GetByIdAsync(id);

            if (!await Common.IsAuthorizedAsync(userToDelete, loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            } 
            
            var accountToDelete = await this.accountRepository.GetByIdAsync((int)userToDelete.AccountId);
            await this.accountService.DeleteAsync(accountToDelete.Id, loggedUser);

            return await this.userRepository.DeleteAsync(id);
        }
        public async Task<User> PromoteAsync(int id, User loggedUser)
        {
            if (!await Common.IsAdminAsync(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }
            return await this.userRepository.PromoteAsync(id);
        }
        public async Task<User> BlockUserAsync(int id, User loggedUser)
        {
            if (!await Common.IsAdminAsync(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }
            return await this.userRepository.BlockUserAsync(id);
        }
        public async Task<User> UnblockUserAsync(int id, User loggedUser)
        {
            if (!await Common.IsAdminAsync(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }
            return await this.userRepository.UnblockUserAsync(id);
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await this.userRepository.EmailExistsAsync(email);
        }
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await this.userRepository.UsernameExistsAsync(username);
        }
        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
        {
            return await this.userRepository.PhoneNumberExistsAsync(phoneNumber);
        }
    }
}
