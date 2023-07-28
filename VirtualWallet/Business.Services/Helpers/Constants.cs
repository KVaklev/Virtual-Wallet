﻿namespace Business.Services.Helpers
{
    public class Constants
    {
        //Constants for user

        public const string ModifyUserErrorMessage = "Only an admin is authorized to perform the specified action.";
        public const string ModifyUsernameErrorMessage = "Username change is not allowed.";

        public const string ModifyUnauthorizeErrorMessage = "You are not authorized.";
        public const string ModifyTransactionAmountErrorMessage = "Not enough balance.";
        public const string ModifyTransactionNoDataErrorMessage = "No data with these parameters.";
        public const string ModifyTransactionDeleteErrorMessage = "You can't delete a completed transaction!";
        public const string ModifyTransactionUpdateErrorMessage = "You can't update a completed transaction.";

        public const string ModifyCurrencyErrorMessage = "You are not admin.";
         
        public const string ModifyAccountErrorMessage = "Only the admin or owner of the account can access, delete or modify it.";
        public const string ModifyAccountCardErrorMessage = "Only the admin or owner of the account can add or remove a card.";

        public const string ModifyCardErrorMessage = "Only the admin or owner of the card can update information.";

        public const string ModifyTransferErrorMessage = "You are not authorized to create, delete or modify the transfer.";

        public const string ModifyTransferAmountErrorMessage = "Insufficient balance.";

        public const string ModifyTransferNoDataErrorMessage = "There are now results related to these parameters";






    }
}
