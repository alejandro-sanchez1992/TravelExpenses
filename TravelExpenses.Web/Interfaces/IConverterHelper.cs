using System;
using System.Collections.Generic;
using TravelExpenses.Common.Models;
using TravelExpenses.Web.Data.Entities;

namespace TravelExpenses.Web.Interfaces
{
    public interface IConverterHelper
    {
        EmployeeResponse ToUserResponse(EmployeeEntity employee);

        List<TripResponse> ToTripResponse(List<TripEntity> tripEntity);

        TripResponse ToTripResponse(TripEntity tripEntity);

        TripDetailResponse ToTripDetailResponse(TripDetailEntity tripDetailEntity);
    }
}
