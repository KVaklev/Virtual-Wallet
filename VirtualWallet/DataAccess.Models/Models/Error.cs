namespace DataAccess.Models.Models
{
    public class Error
    {
        public Error(string invalidPropertyName)
        {
            this.InvalidPropertyName = invalidPropertyName;
        }
        public string InvalidPropertyName { get; set; }


    }
}
