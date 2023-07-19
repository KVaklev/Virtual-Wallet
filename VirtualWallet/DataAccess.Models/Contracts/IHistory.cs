namespace DataAccess.Models.Contracts
{
    public interface IHistory
    {
        public int Id { get; set; }    
        public string Description { get; }
        public DateTime EventTime { get; }
    }
}
