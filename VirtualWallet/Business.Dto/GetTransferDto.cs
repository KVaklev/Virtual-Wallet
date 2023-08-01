using DataAccess.Models.Models;
using DataAccess.Models.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dto
{
    public class GetTransferDto
    {
        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [MinLength(Constants.UsernameMinLength, ErrorMessage = Constants.UsernameMinLengthErrorMessage)]
        [MaxLength(Constants.UsernameMaxLength, ErrorMessage = Constants.UsernameMaxLengthErrorMessage)]
        public string Username { get; set; }

        public DateTime DateCreated { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [StringLength(3, ErrorMessage = "The {0} must be {1} characters long.")]
        public string CurrencyCode { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [CardNumber(ErrorMessage = Constants.CardNumberFieldErroMessage)]
        [StringLength(16, ErrorMessage = Constants.CardNumberLengthErrorMessage)]
        public string CardNumber { get; set; }

        public string TransferType { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        public double Amount { get; set; }
    }
}
