﻿using Business.DTOs.Responses;
using DataAccess.Models.Models;

namespace Business.ViewModels
{
    public class UserDetailsViewModel
    {
        public GetUserDto User {  get; set; }

        public int? Cards { get; set; }
    }
}