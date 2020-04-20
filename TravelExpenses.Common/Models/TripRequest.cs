using System;
using System.ComponentModel.DataAnnotations;

namespace TravelExpenses.Common.Models
{
    public class TripRequest
    {
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public int EmployeeId { get; set; }

        public int City { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
