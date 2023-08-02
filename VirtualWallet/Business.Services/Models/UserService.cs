﻿using Business.Exceptions;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Business.Services.Helpers;
using Business.QueryParameters;
using AutoMapper;
using Business.Mappers;
using Business.DTOs.Requests;
using Business.DTOs.Responses;

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

        public async Task<GetCreatedUserDto> CreateAsync(CreateUserDto createUserDto)
        {     
            if (await UsernameExistsAsync(createUserDto.Username))
            {
                throw new DuplicateEntityException($"User with username '{createUserDto.Username}' already exists.");
            }

            if (await EmailExistsAsync(createUserDto.Email))
            {
                throw new DuplicateEntityException($"User with email '{createUserDto.Email}' already exists.");
            }

            if (await PhoneNumberExistsAsync(createUserDto.PhoneNumber))
            {
                throw new DuplicateEntityException($"User with phone number '{createUserDto.PhoneNumber}' already exists.");
            }
            
            User userToCreate = await UsersMapper.MapCreateDtoToUserAsync(createUserDto);
            userToCreate = await Common.ComputePasswordHashAsync<CreateUserDto>(createUserDto, userToCreate);
            userToCreate = await this.userRepository.CreateAsync(userToCreate);
            await this.accountService.CreateAsync(createUserDto.CurrencyCode, userToCreate);
 
            GetCreatedUserDto createdUser = mapper.Map<GetCreatedUserDto>(userToCreate);

            return createdUser;
        }
        
        public async Task<GetUpdatedUserDto> UpdateAsync(int id, UpdateUserDto updateUserDto, User loggedUser)
        {
            User userToUpdate = await this.userRepository.GetByIdAsync(id);
           
            if (!await Common.IsAuthorizedAsync(userToUpdate, loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }

            if (userToUpdate.Email != updateUserDto.Email)
            {
                if (await EmailExistsAsync(updateUserDto.Email))
                {
                    throw new DuplicateEntityException($"User with email '{updateUserDto.Email}' already exists.");
                }
            }

            if (userToUpdate.PhoneNumber != updateUserDto.PhoneNumber)
            {
                if (await PhoneNumberExistsAsync(updateUserDto.PhoneNumber))
                {
                    throw new DuplicateEntityException($"User with phone number '{updateUserDto.PhoneNumber}' already exists.");
                }
            }

            userToUpdate = await UsersMapper.MapUpdateDtoToUserAsync(userToUpdate, updateUserDto);
            userToUpdate = await Common.ComputePasswordHashAsync<UpdateUserDto>(updateUserDto, userToUpdate);
            userToUpdate = await this.userRepository.UpdateAsync(userToUpdate);

            GetUpdatedUserDto updatedUser = mapper.Map<GetUpdatedUserDto>(userToUpdate);

            return updatedUser;
        }

        public async Task<bool> DeleteAsync(int id, User loggedUser)
        {
            if (!await Common.IsAdminAsync(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            } 
            
            User userToDelete = await this.userRepository.GetByIdAsync(id);
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

            User userToPromote = await this.GetByIdAsync(id);

            if (!userToPromote.IsAdmin)
            {
                userToPromote.IsAdmin = true;
            }

            return await this.userRepository.PromoteAsync(id);
        }

        public async Task<User> BlockUserAsync(int id, User loggedUser)
        {
            if (!await Common.IsAdminAsync(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }

            User userToBlock = await this.GetByIdAsync(id);

            if (!userToBlock.IsBlocked)
            {
                userToBlock.IsBlocked = true;
            }

            return await this.userRepository.BlockUserAsync(id);
        }

        public async Task<User> UnblockUserAsync(int id, User loggedUser)
        {
            if (!await Common.IsAdminAsync(loggedUser))
            {
                throw new UnauthorizedOperationException(Constants.ModifyUserErrorMessage);
            }

            User userToUnblock = await this.GetByIdAsync(id);

            if (userToUnblock.IsBlocked)
            {
                userToUnblock.IsBlocked = false;
            }

            return await this.userRepository.UnblockUserAsync(id);
        }

        private async Task<bool> EmailExistsAsync(string email)
        {
            return await this.userRepository.EmailExistsAsync(email);
        }
        private async Task<bool> UsernameExistsAsync(string username)
        {
            return await this.userRepository.UsernameExistsAsync(username);
        }
        private async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
        {
            return await this.userRepository.PhoneNumberExistsAsync(phoneNumber);
        }

    }
}
