using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TravelExpenses.Web.Interfaces;

namespace TravelExpenses.Web.Helpers
{
    public class CombosHelper : ICombosHelper
    {
        public IEnumerable<SelectListItem> GetComboRoles()
        {
            List<SelectListItem> list = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "[Select a role...]" },
                new SelectListItem { Value = "1", Text = "Employee" },
            };

            return list;
        }
    }
}
