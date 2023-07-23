﻿using System.Security.Principal;

namespace DataAccess.Models.Models
{
    public class Account
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public decimal Balance { get; set; }
        public decimal DailyLimit { get; set; }

        //public string AccountNumber { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public int CardId { get; set; }
        public List<Card> Cards { get; set; }

        //public string InvitationCode { get; set; }
    }
}
