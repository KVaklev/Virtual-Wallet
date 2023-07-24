﻿using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.Models
{
    public class Currency
    {
        public int Id { get; set; }

        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters long.")]
        [MaxLength(30, ErrorMessage = "The {0} must be no more than {1} characters long.")]
        public string Name { get; set; }

        [StringLength(3,ErrorMessage = "The {0} must be {1} characters long.")]
        public string Аbbreviation { get; set; }

        public bool IsDeleted { get; set; }
    }
}
