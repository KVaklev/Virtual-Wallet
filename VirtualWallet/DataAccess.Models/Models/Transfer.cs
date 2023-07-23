﻿using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.Models
{
    public class Transfer
    {

        [Range(1, double.MaxValue)]
        public int Id { get; set; }
        public DateTime Date { get; set; }

        [Range(1, double.MaxValue)]
        public int? UserId { get; set; }
        public User User { get; set; }

        [Range(1, double.MaxValue)]
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        [Range(0, double.MaxValue)]
        public double Amount { get; set; }

        [Range(1, double.MaxValue)]
        public int CardId { get; set; }
        public Card Card { get; set; }

        public bool IsTransferred { get; set; }

        public List<History>? TransferHistories { get; set; } = new List<History>();// possibly to remove
    }
}
