using DataAccess.Models.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs
{
    public class CreateTransferDto
    {
        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [StringLength(3, ErrorMessage = "The {0} must be {1} characters long.")]
        public string CurrencyCode { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [CardNumber(ErrorMessage = Constants.CardNumberFieldErroMessage)]
        [StringLength(16, ErrorMessage = Constants.CardNumberLengthErrorMessage)]
        public string CardNumber {  get; set; }

        public string TransferType { get; set; }    
    }
}
