using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dto
{
    public class CreateTransactionDto
    {
        //todo-validation
        public string RecepiendUsername { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
