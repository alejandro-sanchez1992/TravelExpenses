using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TravelExpenses.Web.Data.Entities
{
    public class ExpenseTypeEntity
    {
        public int Id { get; set; }

        [Display(Name = "Expese Type Name")]
        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Name { get; set; }

        public ICollection<TripDetailEntity> TripDetailEntities { get; set; }
    }
}
