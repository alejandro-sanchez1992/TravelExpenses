﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TravelExpenses.Web.Models.Account
{
    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Document")]
        [MaxLength(20, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string Document { get; set; }

        [Display(Name = "First Name")]
        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public string LastName { get; set; }

        [MaxLength(100, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string Address { get; set; }

        [Display(Name = "Phone Number")]
        [MaxLength(50, ErrorMessage = "The {0} field can not have more than {1} characters.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Picture")]
        public IFormFile PictureFile { get; set; }

        [Display(Name = "Picture")]
        public string PictureFullPath => string.IsNullOrEmpty(PicturePath)
            ? "https://travelexpensesweb.azurewebsites.net/images/noimage.png"
            : $"{PicturePath}";

        [Display(Name = "Picture")]
        public string PicturePath { get; set; }
    }
}
