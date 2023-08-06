using System.Globalization;

namespace DataAccess.Models.ValidationAttributes
{
    public static class Constants
    {
        //Constants for users
        public const int NameMinLength = 2;
        public const int NameMaxLength = 32;
        public const int UsernameMinLength = 2;
        public const int UsernameMaxLength = 20;
        public const int PhoneNumberLength = 10;

        //Constants for cards
        public const int CardHolderMinLength = 2;
        public const int CardHolderMaxLength = 30;
        public const int CheckNumberLength = 3;
       
        //Constants for currencies
        public const int CurrencyNameMinLength = 2;
        public const int CurrencyNameMaxLength = 30;
        public const int CurrencyCodeLength = 3;

        public const int IdMin = 1;
        public const int IdMax = int.MaxValue;
        public const int MinAmount = 1;
        public const int MaxAmount = 10000;

        public const int DescriptionMinLength = 5;
        public const int DescriptionMaxLength = 100;

        //Messages for users
        public const string EmailFieldErrorMessage = "Please provide a valid email.";
        public const string PhoneNumberFieldErroMessage = "The {0} must contain only digits.";
        public const string ImageFileFieldErrorMessage = "Upload File";

        //Messages for cards
        public const string CardNumberFieldErroMessage = "The {0} must contain only digits.";
        public const string CardCheckNumberFieldErrorMessage = "The {0} must contain only digits.";
        public const string EmptyFieldCardTypeErrorMessage = "The field is required. Specify 'Debit' or 'Credit' card ";
        public const string ExpirationDateErrorMessage = "Expiration date must not be in the past.";

        ////Message for tranfer
        public const string EmptyFieldTransferTypeErrorMessage = 
            "The field is required. Specify 'Deposit' or 'Withdrawal' tranfer ";
        public const string TransferIsConfirmedErrorMessage = "Transfer is confirmed! You are not authorized to modify it";
        public const string TransferDirectionInputError = "Transfer direction should be either Deposit or Withdrawal.";


        //Common
        public const string NoFoundErrorMessage = "No matches found.";
        public const string EmptyFieldErrorMessage = "The field is required.";
        public const string RangeFieldErrorMessage = "The {0} field must be in the range from {1} to {2}.";
        public const string MinLengthErrorMessage = "The {0} must be at least {1} characters long.";
        public const string MaxLengthErrorMessage = "The {0} must be no more than {1} characters long.";
        public const string LengthErrorMessage = "The {0} must be exactly {1} characters long.";
    }
}
