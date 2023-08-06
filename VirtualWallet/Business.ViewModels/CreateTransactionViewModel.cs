using Business.DTOs.Requests;
using DataAccess.Models.Models.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels
{
    public class CreateTransactionViewModel
    {
        public CreateTransactionDto CreateTransactionDto { get; set; }

        public SelectList Curencies { get; set; }
    }
}
