using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TravelExpenses.Web.Data.Entities
{
    public class CountryEntity
    {
        public int Id { get; set; }

        [Display(Name = "Country Name")]
        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Name { get; set; }

        [Display(Name = "Short Code")]
        [MaxLength(2, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string ShortCode { get; set; }

        public ICollection<CityEntity> Cities { get; set; }
    }
}
