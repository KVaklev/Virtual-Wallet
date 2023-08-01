namespace DataAccess.Models.ValidationAttributes
{
    public static class Constants
    {
        //Constants for users
        public const int NameMinLength = 2;
        public const int NameMaxLength = 32;
        public const string NameMinLengthErrorMessage = "The {0} must be at least {1} characters long.";
        public const string NameMaxLengthErrorMessage = "The {0} must be no more than {1} characters long.";

        public const int UsernameMinLength = 2;
        public const int UsernameMaxLength = 20;
        public const string UsernameMinLengthErrorMessage = "The {0} must be at least {1} characters long.";
        public const string UsernameMaxLengthErrorMessage = "The {0} must be no more than {1} characters long.";

        public const int PhoneNumberLength = 10;
        public const string PhoneNumberLengthErrorMessage = "The {0} must be exactly {1} characters long.";

        public const string EmailFieldErrorMessage = "Please provide a valid email.";
        public const string PhoneNumberFieldErroMessage = "The {0} must contain only digits.";
        public const string ImageFileFieldErrorMessage = "Upload File";

        //Constants for cards
        public const int CardHolderMinLength = 2;
        public const int CardHolderMaxLength = 30;
        public const string CardHolderMinLengthErrorMessage = "The {0} must be at least {1} characters long.";
        public const string CardHolderMaxLengthErrorMessage = "The {0} must be no more than {1} characters long.";
        
        public const string CardNumberFieldErroMessage = "The {0} must contain only digits.";
        public const string CardNumberLengthErrorMessage = "The {0} must be exactly {1} characters long.";

        public const string CardCheckNumberFieldErrorMessage = "The {0} must contain only digits.";
        public const int CheckNumberLength = 3;
        public const string CheckNumberLengthErrorMessage = "The {0} must be exactly {1} characters long.";

        //Constants for currencies
        public const int CurrencyNameMinLength = 2;
        public const int CurrencyNameMaxLength = 30;
        public const string CurrencyMinLengthErrorMessage = "The {0} must be at least {1} characters long.";
        public const string CurrencyMaxLengthErrorMessage = "The {0} must be no more than {1} characters long.";

        public const int CurrencyCodeLength = 3;
        public const string CurrencyCodeLengthErrorMessage = "The {0} must be {1} characters long.";

        //Others
        public const string EmptyFieldErrorMessage = "The field is required.";
        public const string EmptyFieldCardTypeErrorMessage = "The field is required. Specify 'Debit' or 'Credit' card ";
        public const string EmptyFieldTransferTypeErrorMessage = "The field is required. Specify 'Deposit' or 'Withdrawal' tranfer ";
        public const string EmptyFieldTransactionNameErrorMessage = "The field is required. Specify 'Transaction' or 'Transfer'.";
        public const string EmptyFieldTransactionDirectionErrorMessage = "The field is required. Specify 'In' or 'Out' direction";
        public const string EmptyFieldExpirationDateErrorMessage = "Expiration date is required.";
        public const string ExpirationDateErrorMessage = "Expiration date must not be in the past.";

    }
}
