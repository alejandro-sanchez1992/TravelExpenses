using System;
using System.ComponentModel.DataAnnotations;
using TravelExpenses.Web.Data.Entities;

namespace TravelExpenses.Web.Models.Country
{
    public class CityViewModel : CityEntity
    {
        public int CountryId { get; set; }
    }
}
