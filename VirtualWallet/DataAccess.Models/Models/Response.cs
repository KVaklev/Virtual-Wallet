namespace DataAccess.Models.Models
{
    public class Response<TItem>
    {
        public bool IsSuccessful { get; set; } = true;

        public string Message { get; set; } = string.Empty;

        public TItem Data { get; set; }

        public Error Error { get; set; }

    }
}

