namespace Business.DTOs
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
