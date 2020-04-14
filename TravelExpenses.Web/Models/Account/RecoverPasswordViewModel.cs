using System;
using System.ComponentModel.DataAnnotations;

namespace TravelExpenses.Web.Models.Account
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
