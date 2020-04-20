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
                Trips = employee.Trips?.Select(t => new TripResponse
                {
                    Id = t.Id,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate,
                    TripDetails = t.TripDetails?.Select(td => new TripDetailResponse
                    {
                        Date = td.Date,
                        Id = td.Id,
                        Description = td.Description,
                        Amount = td.Amount,
                        ExpenseType = td.Expense.Name,
                        PicturePath = td.PicturePath

                    }).ToList()
                }).ToList()
            };
        }

        public TripResponse ToTripResponse(TripEntity tripEntity)
        {
            return new TripResponse
            {
                EndDate = tripEntity.EndDate,
                Id = tripEntity.Id,
                StartDate = tripEntity.StartDate,
                TripDetails = tripEntity.TripDetails?.Select(td => new TripDetailResponse
                {
                    Date = td.Date,
                    Id = td.Id,
                    Description = td.Description,
                    Amount = td.Amount,
                    ExpenseType = td.Expense.Name,
                    PicturePath = td.PicturePath
                }).ToList()
            };
        }
    }
}
