using System;
using System.Collections.Generic;

namespace TravelExpenses.Common.Models
{
    public class CountryResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<CityResponse> Cities { get; set; }
    }
}
