using System;

namespace TravelExpenses.Common.Models
{
    public class TripDetailResponse
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public DateTime DateLocal => Date.ToLocalTime();

        public string Description { get; set; }

        public double Amount { get; set; }

        public string ExpenseType { get; set; }

        public string PicturePath { get; set; }
    }
}