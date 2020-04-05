using System;
using System.Linq;
using System.Threading.Tasks;
using TravelExpenses.Common.Enums;
using TravelExpenses.Web.Data.Entities;
using TravelExpenses.Web.Interfaces;

namespace TravelExpenses.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;

        public SeedDb(
            DataContext dataContext,
            IUserHelper userHelper)
        {
            _dataContext = dataContext;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1020451189", "Alejandro", "Sanchez", "alejosanosp@gmail.com", "316 211 8090", "Calle Falsa 123", UserType.Admin);
            UserEntity employee = await CheckUserAsync("1020345678", "Alejandro", "Sanchez", "alejosanosp@hotmail.com", "316 211 8090", "Calle Falsa 123", UserType.Employee);
            await CheckEmployeeAsync(employee);
        }

        private async Task<UserEntity> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            UserType userType)
        {
            UserEntity user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new UserEntity
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    UserType = userType
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.Employee.ToString());
        }

        private async Task CheckEmployeeAsync(UserEntity user)
        {
            if (!_dataContext.Employees.Any())
            {
                _dataContext.Employees.Add(new EmployeeEntity { User = user });
                await _dataContext.SaveChangesAsync();
            }
        }
    }
}
