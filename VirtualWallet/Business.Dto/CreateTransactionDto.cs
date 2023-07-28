namespace Business.Dto
{
    public class CreateTransactionDto
    {
        //todo-validation
        public string RecepientUsername { get; set; }
        public decimal Amount { get; set; }
        public string Abbreviation { get; set; }
    }
}
