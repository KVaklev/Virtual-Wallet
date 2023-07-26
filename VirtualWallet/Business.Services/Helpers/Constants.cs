﻿namespace Business.Services.Helpers
{
    public class Constants
    {
        //Constants for user

        public const string ModifyUserErrorMessage = "Only an admin is authorized to perform the specified action.";
        public const string ModifyUsernameErrorMessage = "Username change is not allowed.";
        public const string ModifyTransactionErrorMessage = "You are not authorized.";
        public const string ModifyTransactionAmountErrorMessage = "Not enough balance.";
        public const string ModifyCurrencyErrorMessage = "You are not admin.";

       
    }
}
