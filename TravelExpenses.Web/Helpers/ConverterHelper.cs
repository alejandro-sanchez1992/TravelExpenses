using System;
using System.Collections.Generic;
using System.Linq;
using TravelExpenses.Common.Models;
using TravelExpenses.Web.Data.Entities;
using TravelExpenses.Web.Interfaces;

namespace TravelExpenses.Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public EmployeeResponse ToUserResponse(EmployeeEntity employee)
        {
            if (employee == null)
            {
                return null;
            }

            return new EmployeeResponse
            {
                Id = employee.Id,
                User = new  UserResponse
                {
                    Address = employee.User.Address,
                    Document = employee.User.Document,
                    Email = employee.User.Email,
                    FirstName = employee.User.FirstName,
                    Id = employee.User.Id,
                    LastName = employee.User.LastName,
                    PhoneNumber = employee.User.PhoneNumber,
                    PicturePath = employee.User.PicturePath,
                    UserType = employee.User.UserType
                },
                Trips = employee.Trips?.Select(t => ToTripResponse(t)).ToList(),
            };
        }

        public List<TripResponse> ToTripResponse(List<TripEntity> tripEntity)
        {
            return tripEntity.Select(t => new TripResponse
            {
                Id = t.Id,
                EndDate = t.EndDate,
                StartDate = t.StartDate,
                City = new CityResponse
                {
                    Id = t.City.Id,
                    Name = t.City.Name
                },
                TripDetails = t.TripDetails?.Select(td => ToTripDetailResponse(td)).ToList()
            }).ToList();
        }

        public TripResponse ToTripResponse(TripEntity tripEntity)
        {
            return new TripResponse
            {
                Id = tripEntity.Id,
                EndDate = tripEntity.EndDate,
                StartDate = tripEntity.StartDate,
                City = new CityResponse {
                    Id = tripEntity.City.Id,
                    Name = tripEntity.City.Name
                },
                TripDetails = tripEntity.TripDetails?.Select(td => ToTripDetailResponse(td) ).ToList()
            };
        }

        public TripDetailResponse ToTripDetailResponse(TripDetailEntity tripDetailEntity)
        {
            return new TripDetailResponse
            {
                Date = tripDetailEntity.Date,
                Id = tripDetailEntity.Id,
                Description = tripDetailEntity.Description,
                Amount = tripDetailEntity.Amount,
                ExpenseType = new ExpenseTypeResponse
                {
                    Id = tripDetailEntity.Expense.Id,
                    Name = tripDetailEntity.Expense.Name
                },
                PicturePath = tripDetailEntity.PicturePath
            };
        }
    }
}
