using System;
using TravelExpenses.Common.Models;
using TravelExpenses.Web.Data.Entities;

namespace TravelExpenses.Web.Interfaces
{
    public interface IConverterHelper
    {
        EmployeeResponse ToUserResponse(EmployeeEntity employee);

        TripResponse ToTripResponse(TripEntity tripEntity);
    }
}
