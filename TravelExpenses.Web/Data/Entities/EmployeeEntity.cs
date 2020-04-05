using System;
using System.Collections.Generic;

namespace TravelExpenses.Web.Data.Entities
{
    public class EmployeeEntity
    {
        public int Id { get; set; }

        public UserEntity User { get; set; }

        public ICollection<TripEntity> Trips { get; set; }
    }
}
