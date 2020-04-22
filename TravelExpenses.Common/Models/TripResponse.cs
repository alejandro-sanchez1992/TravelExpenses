using System;
using System.Collections.Generic;

namespace TravelExpenses.Common.Models
{
    public class TripResponse
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime StartDateLocal => StartDate.ToLocalTime();

        public DateTime? EndDate { get; set; }

        public DateTime? EndDateLocal => EndDate?.ToLocalTime();

        public double TotalAmount { get; }

        public CityResponse City { get; set; }

        public List<TripDetailResponse> TripDetails { get; set; }
    }
}
