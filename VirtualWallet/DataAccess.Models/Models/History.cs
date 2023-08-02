using DataAccess.Models.Enums;
using DataAccess.Models.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.Models
{
    public class History 
    {
        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [Range(Constants.IdMin, Constants.IdMax, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public int Id { get; set; } 

        public DateTime EventTime { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        [Range(Constants.IdMin, Constants.IdMax, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public int AccountId { get; set; }

        public Account Account { get; set; }

        [Required(ErrorMessage = Constants.EmptyFieldErrorMessage)]
        public NameOperation NameOperation { get; set; }


        [Range(Constants.IdMin, Constants.IdMax, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public int? TransactionId { get; set; }
        public Transaction? Transaction { get; set; }


        [Range(Constants.IdMin, Constants.IdMax, ErrorMessage = Constants.RangeFieldErrorMessage)]
        public int? TransferId { get; set; }
        public Transfer? Transfer { get; set; }

    }
}
