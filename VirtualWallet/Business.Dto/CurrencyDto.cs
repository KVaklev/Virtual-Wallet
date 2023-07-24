using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dto
{
    public class CurrencyDto
    {
        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters long.")]
        [MaxLength(30, ErrorMessage = "The {0} must be no more than {1} characters long.")]
        public string Name { get; set; }

        [StringLength(3, ErrorMessage = "The {0} must be {1} characters long.")]
        public string Аbbreviation { get; set; }
    }
}
