using DataAccess.Models.Contracts;

namespace DataAccess.Models.Models
{
    public class History : IHistory
    {
        public int Id { get; set; } 
        public string Description { get; set; }
        public DateTime EventTime { get; set; }

    }
}
