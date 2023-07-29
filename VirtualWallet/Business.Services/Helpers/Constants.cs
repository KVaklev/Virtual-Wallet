namespace Business.Services.Helpers
{
    public class Constants
    {
        //Constant messages for user

        public const string ModifyUserErrorMessage = "Only an admin is authorized to perform the specified action.";
        public const string ModifyUsernameErrorMessage = "Username change is not allowed.";

        //Constant messages for account

        public const string ModifyAccountErrorMessage = "Only an admin or owner of the account can access, delete or modify the acount.";
        public const string ModifyAccountCardErrorMessage = "Only an admin or owner of the account can add or remove a card.";

        //Constant messages for cards

        public const string ModifyCardErrorMessage = "Only an admin or owner of the card can update information.";

        //Constant messages for transactions 

        public const string ModifyTransactionAmountErrorMessage = "Not enough balance.";
        public const string ModifyTransactionNoDataErrorMessage = "No data with these parameters.";
        public const string ModifyTransactionDeleteErrorMessage = "You can't delete a completed transaction!";
        public const string ModifyTransactionUpdateErrorMessage = "You can't update a completed transaction!";
        public const string ModifyTransactionBlockedErrorMessage = "You are not allowed to make transactions while being blocked!";
        public const string ModifyTransactionErrorMessage = "Only an admin or sender of the transaction is authorized to perform the specified action.";

        //Constant messages for transfers

        public const string ModifyTransferAmountErrorMessage = "Insufficient balance.";
        public const string ModifyTransferErrorMessage = "You are not authorized to create, delete or modify the transfer.";
        public const string ModifyTransferNoDataErrorMessage = "There are now results related to these parameters";
        public const string ModifyTransferGetByIdErrorMessage = "You are not authorized for the specified action.";


        //Constant messages for currency

        public const string ModifyCurrencyErrorMessage = "You are not admin.";

    }
}
