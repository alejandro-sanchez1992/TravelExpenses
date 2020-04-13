using System;
using System.Collections.Generic;
using TravelExpenses.Common.Models;

namespace TravelExpenses.Prism.Helpers
{
    public static class CombosHelper
    {
        public static List<Role> GetRoles()
        {
            return new List<Role>
            {
                new Role { Id = 1, Name = Languages.Employee }
            };
        }
    }
}
