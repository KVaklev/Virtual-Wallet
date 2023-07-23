﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dto
{
    public class AccountDto
    {
        public string  Username { get; set; }

        public string AccountNumber { get; set; }

        public string Currency { get; set; }

        public DateTime DateCreated { get; set; }

        public double DailyLimit { get; set; }


    }
}
