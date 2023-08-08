using DataAccess.Models.Models;

namespace Business.QueryParameters
{
    public class UserQueryParameters
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public bool? Admin { get; set; }
        public bool? Blocked { get; set; }
        public int PageSize { get; set; } = 5;
        public int PageNumber { get; set; } = 1;

    }
}
