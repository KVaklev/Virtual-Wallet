using Business.Exceptions;
using Business.Services.Contracts;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Business.Services.Helpers;
using Business.QueryParameters;
using AutoMapper;
using Business.Mappers;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using Business.DTOs;
using static Business.Services.Helpers.Constants;


namespace Business.Services.Models
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IAccountService accountService;
        private readonly IMapper mapper;

        public UserService(
            IUserRepository userRepository,
            IAccountService accountService,
            IMapper mapper
            )
        {
            this.userRepository = userRepository;
            this.accountService = accountService;
            this.mapper = mapper;
        }

        public Response<IQueryable<GetUserDto>> GetAll()
        {
            var result = new Response<IQueryable<GetUserDto>>();
            var users = this.userRepository.GetAll();

            if (users != null && users.Any())
            {
                result.IsSuccessful = true;
                result.Data = (IQueryable<GetUserDto>)users.AsQueryable();
            }
            else
            {
                result.IsSuccessful = false;
                result.Message = NoUsersErrorMessage;
            }

            return result;
        }

        public async Task<PaginatedList<User>> FilterByAsync(UserQueryParameters filterParameters)
        {
            var result = await this.userRepository.FilterByAsync(filterParameters);

            List<GetCreatedUserDto> userDtos = result
                    .Select(user => mapper.Map<GetCreatedUserDto>(user))
                    .ToList();
            return result;
        }

        public async Task<Response<GetUserDto>> GetByIdAsync(int id, User loggedUser)
        {
            var result = new Response<GetUserDto>();

            var user = await this.userRepository.GetByIdAsync(id);

            if (!await Security.IsAuthorizedAsync(user, loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = ModifyAuthorizedErrorMessage;
                return result;
            }
            var userDto = this.mapper.Map<GetUserDto>(user);
            result.Data = userDto;

            return result;
        }

        public async Task<Response<GetUserDto>> GetByUsernameAsync(string username)
        {
            var result = new Response<GetUserDto>();

            var user = await this.userRepository.GetByUsernameAsync(username);
            var userDto = this.mapper.Map<GetUserDto>(user);
            result.Data = userDto;

            return result;
        } 

        public async Task<Response<GetCreatedUserDto>> CreateAsync(CreateUserModel createUserDto)
        {
            var result = new Response<GetCreatedUserDto>();

            if (await UsernameExistsAsync(createUserDto.Username))
            {
                result.IsSuccessful = false;
                result.Message = UsernameExistsErrorMessage;
                result.Error = new Error(PropertyName.Username);
                return result;
            }

            if (await EmailExistsAsync(createUserDto.Email))
            {
                result.IsSuccessful = false;
                result.Message = EmailExistsErrorMessage;
                result.Error = new Error(PropertyName.Email);
                return result;
            }

            if (await PhoneNumberExistsAsync(createUserDto.PhoneNumber))
            {
                result.IsSuccessful = false;
                result.Message = PhoneNumberExistsErrorMessage;
                result.Error = new Error(PropertyName.PhoneNumber);
                return result;
            }
            
            User userToCreate = await UsersMapper.MapCreateDtoToUserAsync(createUserDto);
            userToCreate = await Security.ComputePasswordHashAsync<CreateUserModel>(createUserDto, userToCreate);
            userToCreate = await this.userRepository.CreateAsync(userToCreate);
            await this.accountService.CreateAsync(createUserDto.CurrencyCode, userToCreate);
 
            result.Data = mapper.Map<GetCreatedUserDto>(userToCreate);

            return result;
        }
        
        public async Task<Response<GetUpdatedUserDto>> UpdateAsync(int id, UpdateUserDto updateUserDto, User loggedUser)
        {
            var result = new Response<GetUpdatedUserDto>();

            User userToUpdate = await this.userRepository.GetByIdAsync(id);
           
            if (!await Security.IsAuthorizedAsync(userToUpdate, loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = ModifyUserErrorMessage;
                return result;
            }

            if (userToUpdate.Email != updateUserDto.Email)
            {
                if (await EmailExistsAsync(updateUserDto.Email))
                {
                    result.IsSuccessful = false;
                    result.Message = EmailExistsErrorMessage;
                    result.Error = new Error(PropertyName.Email);
                    return result;
                }
            }

            if (userToUpdate.PhoneNumber != updateUserDto.PhoneNumber)
            {
                if (await PhoneNumberExistsAsync(updateUserDto.PhoneNumber))
                {
                    result.IsSuccessful = false;
                    result.Message = PhoneNumberExistsErrorMessage;
                    result.Error = new Error(PropertyName.PhoneNumber);
                    return result;
                }
            }

            userToUpdate = await UsersMapper.MapUpdateDtoToUserAsync(userToUpdate, updateUserDto);
            userToUpdate = await Security.ComputePasswordHashAsync<UpdateUserDto>(updateUserDto, userToUpdate);
            userToUpdate = await this.userRepository.UpdateAsync(userToUpdate);

            result.Data = mapper.Map<GetUpdatedUserDto>(userToUpdate);

            return result;
        }

        public async Task<Response<bool>> DeleteAsync(int id, User loggedUser)
        {
            var result = new Response<bool>();

            if (!await Security.IsAdminAsync(loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = ModifyUserErrorMessage;
                return result;
            } 
            
            User userToDelete = await this.userRepository.GetByIdAsync(id);
            await this.accountService.DeleteAsync((int)userToDelete.AccountId, loggedUser);
    
            result.Data = await this.userRepository.DeleteAsync(id);
            
            return result;
        }

        public async Task<Response<GetUserDto>> PromoteAsync(int id, User loggedUser)
        {
            var result = new Response<GetUserDto>();

            if (!await Security.IsAdminAsync(loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = ModifyUserErrorMessage;
                return result;
            }

            User userToPromote = await this.userRepository.GetByIdAsync(id);

            if (!userToPromote.IsAdmin)
            {
                userToPromote.IsAdmin = true;
            }

            userToPromote = await this.userRepository.PromoteAsync(id);
            result.Data = mapper.Map<GetUserDto>(userToPromote);

            return result;
        }

        public async Task<Response<GetUserDto>> BlockUserAsync(int id, User loggedUser)
        {
            var result = new Response<GetUserDto>();

            if (!await Security.IsAdminAsync(loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = ModifyUserErrorMessage;
                return result;
            }

            User userToBlock = await this.userRepository.GetByIdAsync(id);

            if (!userToBlock.IsBlocked)
            {
                userToBlock.IsBlocked = true;
            }

            userToBlock = await this.userRepository.BlockUserAsync(id);
            result.Data = mapper.Map<GetUserDto>(userToBlock);

            return result;
        }

        public async Task<Response<GetUserDto>> UnblockUserAsync(int id, User loggedUser)
        {
            var result = new Response<GetUserDto>();

            if (!await Security.IsAdminAsync(loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = ModifyUserErrorMessage;
                return result;
            }

            User userToUnblock = await this.userRepository.GetByIdAsync(id);

            if (userToUnblock.IsBlocked)
            {
                userToUnblock.IsBlocked = false;
            }

            userToUnblock = await this.userRepository.UnblockUserAsync(id);
            result.Data = mapper.Map<GetUserDto>(userToUnblock);

            return result;
        }

        public async Task<Response<User>> LoginAsync(string username, string password)
        {
            var result = new Response<User>();

            await Security.CheckForNullEntryAsync(username, password);

            User loggedUser = await this.userRepository.GetByUsernameAsync(username);

           var authenticatedUser = await Security.AuthenticateAsync(loggedUser, password);
           return authenticatedUser;

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
