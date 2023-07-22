﻿using DataAccess.Models.Models;

namespace Business.QueryParameters
{
    public class UserQueryParameters
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }

    }
}