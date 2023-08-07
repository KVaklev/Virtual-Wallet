namespace Business.Services.Helpers
{
    public class Constants
    {
        //Constant messages for user

        public const string ModifyUserErrorMessage = "Only an admin is authorized to perform the specified action.";
        public const string ModifyUsernameErrorMessage = "Username change is not allowed.";

        public const string NoUsersErrorMessage = "No users are found.";
        public const string NoUsersAfterFilterErrorMessage = "No users match the specified filter criteria.";

        public const string UsernameExistsErrorMessage = "User with this username already exists.";
        public const string UsernameDoesntExistErrorMessage = "User with this username doesn't exist.";
        public const string EmailExistsErrorMessage = "User with this email already exists.";
        public const string PhoneNumberExistsErrorMessage = "User with this phone number already exists.";

        public const string SuccessfullDeletedUserMessage = "User was successfully deleted.";
        public const string SuccessfullPromoteddUserMessage = "User was successfully promoted with admin rights.";
        public const string SuccessfullBlockedUserMessage = "User was successfully blocked.";
        public const string SuccessfullUnblockedUserMessage = "User was successfully unblocked.";

        //Constant messages for account

        public const string NoAccountsErrorMessage = "No accounts are found.";
        public const string ModifyAccountErrorMessage = "Only an admin or owner of the account can access, delete or modify the acount.";
        public const string ModifyAccountCardErrorMessage = "Only an admin or owner of the account can add or remove a card.";
        public const string ModifyAccountBalancetErrorMessage = "Insufficient balance.";

        //Constant messages for cards

        public const string NoCardsErrorMessage = "No cards are found.";
        public const string NoCardsByAccountSearchErrorMessage = "Account with this ID does not have any cards.";
        public const string ModifyCardErrorMessage = "Only an admin or owner of the card can access, delete or modify the card.";
        public const string CardNumberAddErrorMessage = "Card with the specified card number has already been registered.";
        public const string SuccessfullDeletedCardMessage = "Card has been successfully deleted.";
        public const string NoCardFoundErrorMessage = "No card is found.";

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
        public const string ModifyDeleteTransfer = "The transfer has been successfully deleted.";
        public const string ModifyExecutedTransfer = "The transfer has been successfully confirmed.";

        //Constant messages for curencies

        public const string ModifyCurrencyDeleteMessage = "The currency has been successfully deleted.";
        public const string CurrencyNotFoundErrorMessage = "This currency does not exist.";

        //Constant messages for Authorized

        public const string ModifyAuthorizedErrorMessage = "You are not authorized for the specified action.";

        //Constant messages for ExchangeRate

        public const string ModifyHttpRequestExceptionErrorMessage = "No such host is known.";
        public const string ModifyJsonExceptionErrorMessage = "JSON Deserialization Error.";

        //Constant messages for token

        public const string GenerateTokenErrorMessage = "An error occurred in generating the token";
        public const string SuccessfullTokenMessage = "Logged in successfully. Token: ";
        public const string SuccessfullLoggedInMessage = "Logged in successfully!";

        //Constant messages for registration 

        public const string ConfirmedRegistrationMessage = "Registration confirmed!";
        public const string NotSuccessfullRegistrationMessage = "Your registration was not successfull.";
        public const string SuccessfullConfirmationEmailSentMessage = "Hooray! Your confirmation email has been sent successfully. Check your inbox, follow the link to complete your registration, and then dive right in to create your wallet!";
        public const string InvalidSendEmailOperation = "An error occurred while sending the email. Please try again later.";

        //Constant messages for login

        public const string CredentialsErrorMessage = "Username and/or Password not specified";
        public const string FailedLoginAtemptErrorMessage = "Nice try! Invalid credentials!";
        public const string NotConfirmedEmailErrorMessage = "Your email is not confirmed, please check your inbox folder and follow the link!";

        public static class PropertyName
        {
            public const string Username = "Username";
            public const string Email = "Email";
            public const string PhoneNumber = "PhoneNumber";
            public const string CardNumber = "CardNumber";
            public const string Credentials = "Credentials";
            public const string PasswordHashMatch = "PasswordHashMatch";
            public const string NotConfirmedEmail = "NotConfirmedEmail";
            public const string UsernameDoesntExist = "UsernameDoesntExist";
        }

        public const string NoFoundResulte = "No found result.";
        public const string NoHostError= "No such host is known.";
        public const string JsonDeserializationError = "JSON Deserialization Error";

    }


}
