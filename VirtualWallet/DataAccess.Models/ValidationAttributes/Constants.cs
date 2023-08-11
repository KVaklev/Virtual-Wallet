
namespace DataAccess.Models.ValidationAttributes
{
    public static class Constants //ToDo - check for repeting messages
    {
        //Constants for users

        public const int NameMinLength = 2;
        public const int NameMaxLength = 32;
        public const int UsernameMinLength = 2;
        public const int UsernameMaxLength = 20;
        public const int PhoneNumberLength = 10;
        public const int AddressMaxLength = 150;
        public const int CountryMaxLength = 25;
        public const int CityMaxLength = 85;

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

        //Messages for password validation

        public const string PasswordMinLengthErrorMessage = "Password must be at least 8 characters long.";
        public const string PasswordLowerCaseErrorMessage = "Password must contain at least one lowercase letter.";
        public const string PasswordUpperCaseErrorMessage = "Password must contain at least one uppercase letter.";
        public const string PasswordDigitContainErrorMessage = "Password must contain at least one digit.";
        public const string PasswordSpecialCharacterContainErrorMessage = "Password must contain at least one special character.";

        //Messages for phone number validation

        public const string PhoneNumberEmptyFieldErrorMessage = "The phone number must not be empty.";
        public const string PhoneNumberDigitContainErrorMessage = "The phone number must contain only digits.";

        //Messages for users

        public const string EmailFieldErrorMessage = "Please provide a valid email.";
        public const string PhoneNumberFieldErroMessage = "The {0} must contain only digits.";
        public const string ImageFileFieldErrorMessage = "Upload File";
        public const string UsernameDoesntExistErrorMessage = "User with this username does not exist.";
        public const string UserWithIdDoesntExistErrorMessage = "User with this ID does not exist.";
        public const string NoUsersErrorMessage = "No users are found.";
        public const string NoUsersAfterFilterErrorMessage = "No users match the specified filter criteria.";
        public const string BirthDateErrorMessage = "Nice try, time traveller! We're not ready for tomorrow's babies. Enter a valid birthdate!";

        //Messages for cards

        public const string CardNumberFieldErroMessage = "The {0} must contain only digits.";
        public const string CardCheckNumberFieldErrorMessage = "The {0} must contain only digits.";
        public const string EmptyFieldCardTypeErrorMessage = "The field is required. Specify 'Debit' or 'Credit' card ";
        public const string ExpirationDateErrorMessage = "Expiration date must not be in the past.";

        //Messages for tranfer

        public const string EmptyFieldTransferTypeErrorMessage = "The field is required. Specify 'Deposit' or 'Withdrawal' tranfer ";
        public const string TransferIsConfirmedErrorMessage = "Transfer is confirmed! You are not authorized to modify it";
        public const string TransferDirectionInputError = "Transfer direction should be either Deposit or Withdrawal.";

        //Messages for accounts

        public const string NoAccountsErrorMessage = "No accounts are found.";
        public const string AccountWithIdDoesntExistErrorMessage = "Account with this ID does not exist.";
        public const string AccountWithUsernameDoesntExistErrorMessage = "Account with this username does not exist.";

        //Common

        public const string NotFoundErrorMessage = "No matches found.";
        public const string EmptyFieldErrorMessage = "The field is required.";
        public const string RangeFieldErrorMessage = "The {0} field must be in the range from {1} to {2}.";
        public const string MinLengthErrorMessage = "The {0} must be at least {1} characters long.";
        public const string MaxLengthErrorMessage = "The {0} must be no more than {1} characters long.";
        public const string LengthErrorMessage = "The {0} must be exactly {1} characters long.";
    }
}
