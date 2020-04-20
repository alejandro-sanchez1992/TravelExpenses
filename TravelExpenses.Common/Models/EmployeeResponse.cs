using System;
using System.Collections.Generic;

namespace TravelExpenses.Common.Models
{
    public class EmployeeResponse
    {
        public int Id { get; set; }

        public UserResponse User { get; set; }

        public List<TripResponse> Trips { get; set; }
    }
}
