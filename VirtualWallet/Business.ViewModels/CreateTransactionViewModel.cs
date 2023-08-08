﻿
using Business.DTOs.Requests;
using DataAccess.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Business.ViewModels
{
    public class CreateTransactionViewModel
    {
        public CreateTransactionDto CreateTransactionDto { get; set; }

        public List<CreateCurrencyDto> Currencies { get; set; }
    }
}
