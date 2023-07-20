﻿using DataAccess.Models.Contracts;

namespace Business.Services.Contracts
{
    public interface IUserService
    {
        List<IUser> GetAll();
       // PaginatedList<IUser> FilterBy(UserQueryParameters filterParameters);
        IUser GetById(int id);
        IUser GetByUsername(string username);
        IUser GetByEmail(string email);
        IUser GetByPhoneNumber(int phoneNumber);
        IUser Create(IUser user);
        IUser Update(int id, IUser user, IUser loggedUser);
        void Delete(int id, IUser loggedUser);
        IUser Promote(IUser user);
        IUser BlockUser(IUser user);
        IUser UnblockUser(IUser user); 
        bool UsernameExists(string username);
        bool EmailExists(string email);
        bool PhoneNumberExists(int phoneNumber);
        bool IsAuthorized(IUser user, IUser loggedUser);
    }
}
