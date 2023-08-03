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
        public const string ModifyAccountBalancetErrorMessage = "Insufficient balance.";

        //Constant messages for cards

        public const string ModifyCardErrorMessage = "Only an admin or owner of the card can access, delete or modify the card.";

        //Constant messages for transactions 

        public const string ModifyTransactionExecuteMessage = "The transaction has been successfully executed.";
        public const string ModifyTransactionDeleteMessage = "The transacion has been successfully deleted.";
        public const string ModifyTransactionNotExecuteErrorMessage = "You cannot update or delete a completed transaction!";
        public const string ModifyTransactionBlockedErrorMessage = "You are not allowed to make transactions while being blocked!";
        

        //Constant messages for transfers

        public const string ModifyTransferErrorMessage = "You are not authorized to create, delete or modify the transfer.";
        public const string ModifyTransferNoDataErrorMessage = "There are no results related to these parameters.";
        public const string ModifyTransferGetByIdErrorMessage = "You are not authorized for the specified action.";
        public const string ModifyTransferUpdateDeleteErrorErrorMessage = "The transfer is either processed or cancelled so you cannot update or delete it.";

        //Constant messages for curencies

        public const string ModifyCurrencyDeleteMessage = "The currency has been successfully deleted.";
        public const string ModifyCurrencyNotFoundErrorMessage = "This currency does not exist.";

        //Constant messages for Authorized

        public const string ModifyAuthorizedErrorMessage = "You are not authorized for the specified action.";

        //Constant messages for ExchangeRate

        public const string ModifyHttpRequestExceptionErrorMessage = "No such host is known.";
        public const string ModifyJsonExceptionErrorMessage = "JSON Deserialization Error.";
    }
}
