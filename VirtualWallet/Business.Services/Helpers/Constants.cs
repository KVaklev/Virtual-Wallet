﻿namespace Business.Services.Helpers
{
    public class Constants
    {
        //Constants for user

        public const string ModifyUserErrorMessage = "Only an admin is authorized to perform the specified action.";
        public const string ModifyUsernameErrorMessage = "Username change is not allowed.";
        public const string ModifyTransactionErrorMessage = "You are not authorized.";
        public const string ModifyAccountErrorMessage = "Only the admin or owner of the account can delete or modify it.";

    }
}
