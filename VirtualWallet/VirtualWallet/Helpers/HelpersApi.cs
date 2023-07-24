using DataAccess.Models.Models;
using Presentation.Helpers;
using AutoMapper;
using Business.Dto;
using Business.Exceptions;
using Business.QueryParameters;
using Business.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VirtualWallet.Helpers
{
    public class HelpersApi : Controller
    {
        private readonly AuthManager authManager;

        public HelpersApi(AuthManager authManager)
        {
            this.authManager = authManager;
        }
        

        public User FindLoggedUser()
        {
            var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
            var loggedUser = authManager.TryGetUserByUsername(loggedUsersUsername);
            return loggedUser;
        }
    }
}
