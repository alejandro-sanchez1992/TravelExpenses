using System;
using System.ComponentModel.DataAnnotations;

namespace TravelExpenses.Web.Data.Entities
{
    public class TripDetailEntity
    {
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = false)]
        public DateTime Date { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = false)]
        public DateTime DateLocal => Date.ToLocalTime();

        public double Amount { get; set; }

        public string Description { get; set; }

        [Display(Name = "Picture")]
        public string PicturePath { get; set; }

        public TripEntity Trip { get; set; }

        public ExpenseTypeEntity Expense { get; set; }
    }
}
