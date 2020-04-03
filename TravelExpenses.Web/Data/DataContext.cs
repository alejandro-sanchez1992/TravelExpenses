using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TravelExpenses.Web.Data.Entities;

namespace TravelExpenses.Web.Data
{
    public class DataContext : IdentityDbContext<UserEntity>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<EmployeeEntity> Employees { get; set; }

        public DbSet<TripEntity> Trips { get; set; }

        public DbSet<TripDetailEntity> TripDetails { get; set; }

        public DbSet<ExpenseTypeEntity> ExpenseTypes { get; set; }

        public DbSet<CityEntity> Cities { get; set; }

        public DbSet<CountryEntity> Countries { get; set; }
    }
}
