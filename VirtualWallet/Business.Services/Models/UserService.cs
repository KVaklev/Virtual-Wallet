using Business.Services.Contracts;
using DataAccess.Models.Models;
using DataAccess.Repositories.Contracts;
using Business.Services.Helpers;
using Business.QueryParameters;
using AutoMapper;
using Business.Mappers;
using Business.DTOs.Requests;
using Business.DTOs.Responses;
using static Business.Services.Helpers.Constants;
using DataAccess.Models.Enums;
using Business.ViewModels;
using Business.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Hosting;

namespace Business.Services.Models
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IAccountService accountService;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment webHostEnvironment;

        public UserService(
            IUserRepository userRepository,
            IAccountService accountService,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment
            )
        {
            this.userRepository = userRepository;
            this.accountService = accountService;
            this.mapper = mapper;
            this.webHostEnvironment = webHostEnvironment;
    }

        public async Task<Response<PaginatedList<GetCreatedUserDto>>> FilterByAsync(UserQueryParameters filterParameters)
        {
            var result = new Response<PaginatedList<GetCreatedUserDto>>();
            var usersResult = this.GetAll();
            IQueryable<User> users = usersResult.Data;

            users = await FilterByFirstNameAsync(users, filterParameters.FirstName);
            users = await FilterByLastNameAsync(users, filterParameters.LastName);
            users = await FilterByUsernameAsync(users, filterParameters.Username);
            users = await FilterByEmailAsync(users, filterParameters.Email);
            users = await FilterByPhoneNumberAsync(users, filterParameters.PhoneNumber);
            users = await FilterByAdminStatusAsync(users, filterParameters.Admin);
            users = await FilterByBlockedStatusAsync(users, filterParameters.Blocked);
            users = await SortByAsync(users, filterParameters.SortBy);
            users = await SortOrderAsync(users, filterParameters.SortOrder);

            int totalPages = (users.Count() + filterParameters.PageSize - 1) / filterParameters.PageSize;
            users = await Common<User>.PaginateAsync(users, filterParameters.PageNumber, filterParameters.PageSize);

            if (!users.Any())
            {
                result.IsSuccessful = false;
                result.Message= NoUsersErrorMessage;
                return result;
            }

            List<GetCreatedUserDto> userDtos = users
                    .Select(user => mapper.Map<GetCreatedUserDto>(user))
                    .ToList();
            result.Data = new PaginatedList<GetCreatedUserDto>(userDtos,totalPages,filterParameters.PageNumber);

            return result;
        }
  
        public async Task<Response<GetUserDto>> GetByIdAsync(int id, User loggedUser)
        {
            var result = new Response<GetUserDto>();

            var user = await this.userRepository.GetByIdAsync(id);
            if (user == null) 
            {
                result.IsSuccessful = false;
                result.Message = NoUsersErrorMessage;
                return result;
            }

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

            if (user == null)
            {
                result.IsSuccessful = false;
                result.Message = NoUsersErrorMessage;
                return result;
            }

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
            if (userToUpdate == null)
            {
                result.IsSuccessful = false;
                result.Message = NoUsersErrorMessage;
                return result;
            }

            if (!await Security.IsAuthorizedAsync(userToUpdate, loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = UpdateStatusUserErrorMessage;
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

        public async Task<Response<bool>> ChangeStatusAsync(int id, UserDetailsViewModel userDetailsViewModel, User loggedUser)
        {
            var result = new Response<bool>();

            if (!await Security.IsAdminAsync(loggedUser))
            {
                result.IsSuccessful = false;
                result.Message = ModifyUserErrorMessage;
                return result;
            }

            User userToEditStatus = await this.userRepository.GetByIdAsync(id);

            userToEditStatus.IsAdmin = userDetailsViewModel.User.Admin;
            userToEditStatus.IsBlocked = userDetailsViewModel.User.Blocked;

            await this.userRepository.SaveChangesAsync();

            return result;
        }
        public async Task<Response<GetUpdatedUserDto>> ChangeProfilePictureAsync(int id, UserDetailsViewModel userDetailsViewModel, User loggedUser)
        {
            var result = new Response<GetUpdatedUserDto>();
           
            var userToUpdate = await this.userRepository.GetByIdAsync(id);
            if (userToUpdate == null)
            {
                result.IsSuccessful = false;
                result.Message = NoUsersErrorMessage;
                return result;
            }
           
            if (userDetailsViewModel.User.ImageFile != null)
            {
                string imageUploadedFolder = Path.Combine(webHostEnvironment.WebRootPath, "UploadedImages");
                string username = userToUpdate.Username;
                string uniqueFileName = username + ".png";
                string filePath = Path.Combine(imageUploadedFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    userDetailsViewModel.User.ImageFile.CopyTo(fileStream);
                }
                userToUpdate.ProfilePhotoPath = "~/UploadedImages";
                userToUpdate.ProfilePhotoFileName = uniqueFileName;
                userToUpdate.ImageFile = userDetailsViewModel.User.ImageFile;
            }

            result.Data = mapper.Map<GetUpdatedUserDto>(userToUpdate);
            await this.userRepository.SaveChangesAsync();
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
            if (userToDelete == null)
            {
                result.IsSuccessful = false;
                result.Message = NoUsersErrorMessage;
                return result;
            }

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
            if (userToPromote == null)
            {
                result.IsSuccessful = false;
                result.Message = NoUsersErrorMessage;
                return result;
            }

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
            if (userToBlock == null)
            {
                result.IsSuccessful = false;
                result.Message = NoUsersErrorMessage;
                return result;
            }

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
            if (userToUnblock == null)
            {
                result.IsSuccessful = false;
                result.Message = NoUsersErrorMessage;
                return result;
            }

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

            await Common.CheckForNullEntryAsync(username, password);

            User loggedUser = await this.userRepository.GetByUsernameAsync(username);
            if (loggedUser == null)
            {
                result.IsSuccessful = false;
                result.Message = UsernameDoesntExistErrorMessage;
                return result;
            }

            var authenticatedUser = await Security.AuthenticateAsync(loggedUser, password);
           return authenticatedUser;

        }

        public async Task<Response<User>> GetLoggedUserByUsernameAsync(string username)
        {
            var result = new Response<User>();

            var user = await this.userRepository.GetByUsernameAsync(username);
            if (user == null)
            {
                result.IsSuccessful = false;
                result.Message = NoUsersErrorMessage;
                return result;
            }
            result.Data = user;

            return result;
        }

        public async Task<Response<User>> GetLoggedUserByIdAsync(int id)
        {
            var result = new Response<User>();

            var user = await this.userRepository.GetByIdAsync(id);

            if (user == null)
            {
                result.IsSuccessful = false;
                result.Message = NoUserFoundErrorMessage;
                return result;
            }
            result.Data = user;

            return result;
        }
       
        public Response<IQueryable<User>> GetAll()
        {
            var result = new Response<IQueryable<User>>();
            var users = this.userRepository.GetAll();

            if (users.Any())
            {
                result.IsSuccessful = true;
                result.Data = users;
            }
            else
            {
                result.IsSuccessful = false;
                result.Message = NoUsersErrorMessage;
            }

            return result;
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
        private async Task<IQueryable<User>> FilterByFirstNameAsync(IQueryable<User> result, string? firstName)
        {
            if (!string.IsNullOrEmpty(firstName))
            {
                result = result.Where(user => user.Username != null && user.FirstName == firstName);
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<User>> FilterByLastNameAsync(IQueryable<User> result, string? lastName)
        {
            if (!string.IsNullOrEmpty(lastName))
            {
                result = result.Where(user => user.Username != null && user.LastName == lastName);
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<User>> FilterByUsernameAsync(IQueryable<User> result, string? username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                result = result.Where(user => user.Username != null && user.Username == username);
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<User>> FilterByEmailAsync(IQueryable<User> result, string? email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                result = result.Where(user => user.Email != null && user.Email == email);
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<User>> FilterByPhoneNumberAsync(IQueryable<User> result, string? phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                result = result.Where(user => user.PhoneNumber != null && user.PhoneNumber == phoneNumber);
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<User>> FilterByAdminStatusAsync(IQueryable<User> result, bool? isAdmin)
        {
            if (isAdmin.HasValue)
            {
                result = result.Where(user => user.IsAdmin != null && user.IsAdmin == isAdmin);
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<User>> FilterByBlockedStatusAsync(IQueryable<User> result, bool? isBlocked)
        {
            if (isBlocked.HasValue)
            {
                result = result.Where(user => user.IsBlocked != null && user.IsBlocked == isBlocked);
            }
            return await Task.FromResult(result);
        }
        private async Task<IQueryable<User>> SortByAsync(IQueryable<User> result, string? sortCriteria)
        {
            if (Enum.TryParse<SortCriteria>(sortCriteria, true, out var sortEnum))
            {
                switch (sortEnum)
                {
                    case SortCriteria.Username:
                        return await Task.FromResult(result.OrderBy(user => user.Username));
                    case SortCriteria.Email:
                        return await Task.FromResult(result.OrderBy(user => user.Email));
                    case SortCriteria.PhoneNumber:
                        return await Task.FromResult(result.OrderBy(user => user.PhoneNumber));
                }
            }
            return result;
        }
        private async Task<IQueryable<User>> SortOrderAsync(IQueryable<User> result, string? sortOrder)
        {
            if (Enum.TryParse<SortCriteria>(sortOrder, true, out var sortEnum))
            {
                switch (sortEnum)
                {
                    case SortCriteria.Desc:
                        return await Task.FromResult(result.Reverse());
                }
            }
         return await Task.FromResult(result);
        }

    }
}
