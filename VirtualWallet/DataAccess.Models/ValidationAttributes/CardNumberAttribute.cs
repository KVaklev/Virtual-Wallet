using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.ValidationAttributes
{
    public class CardNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var cardNumber = value as string;

            if (string.IsNullOrEmpty(cardNumber))
            {
                return false;
            }

            if (!IsDigitsOnly(cardNumber))
            {
                return false;
            }

            return true;
        }

        private bool IsDigitsOnly(string value)
        {
            foreach (char c in value)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
