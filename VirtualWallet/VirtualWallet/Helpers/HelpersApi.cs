using DataAccess.Models.Models;
using Presentation.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace VirtualWallet.Helpers
{
    public class HelpersApi : ControllerBase
    {
        private readonly AuthManager authManager;

        public HelpersApi(AuthManager authManager)
        {
            this.authManager = authManager;
        }
        

        //public User FindLoggedUser()
        //{
        //    var user = User;
        //    var loggedUsersUsername = User.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
        //    var loggedUser = authManager.TryGetUserByUsername(loggedUsersUsername);
        //    return loggedUser;
        //}
    }
}
