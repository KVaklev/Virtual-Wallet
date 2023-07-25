using System.Numerics;
using System;

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
        public const string PhoneNumberFieldErroMessage = "The phone number must contain only digits.";
        public const string ImageFileFieldErrorMessage = "Upload File";

        //Others
        public const string EmptyFieldErrorMessage = "The field is required.";
    }
}
