using System;
using System.ComponentModel.DataAnnotations;

namespace TravelExpenses.Common.Models
{
    public class TripDetailRequest
    {
        [Required]
        public int TripId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public double Amount { get; set; }

        [MaxLength(500, ErrorMessage = "The {0} field must have {1} characters.")]
        public string Description { get; set; }

        [Required]
        public byte[] PictureArray { get; set; }

        [Required]
        public int ExpenseId { get; set; }
    }
}
