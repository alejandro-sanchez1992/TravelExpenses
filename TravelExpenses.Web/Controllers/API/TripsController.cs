using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelExpenses.Common.Models;
using TravelExpenses.Web.Data;
using TravelExpenses.Web.Data.Entities;
using TravelExpenses.Web.Interfaces;

namespace TravelExpenses.Web.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;

        public TripsController(DataContext context,
            IUserHelper userHelper,
            IImageHelper imageHelper,
            IConverterHelper converterHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
        }

        [HttpPost]
        public async Task<IActionResult> PostTripEntity([FromBody] TripRequest tripRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserEntity userEntity = await _userHelper.GetUserAsync(tripRequest.UserId);
            var employeeEntity = await _context.Employees.FindAsync(tripRequest.EmployeeId);
            if (userEntity == null)
            {
                return BadRequest("User doesn't exists.");
            }

            CityEntity cityEntity = await _context.Cities.FirstOrDefaultAsync(c => c.Id == tripRequest.City);

            TripEntity tripEntity = new TripEntity
            {
                City = cityEntity,
                StartDate = tripRequest.StartDate,
                EndDate = tripRequest.EndDate,
                Employee = employeeEntity,
                User = userEntity
            };

            _context.Trips.Add(tripEntity);
            await _context.SaveChangesAsync();
            return Ok(_converterHelper.ToTripResponse(tripEntity));
        }

        [HttpPost]
        [Route("PostExpenseEntity")]
        public async Task<IActionResult> PostExpenseEntity([FromBody] TripDetailRequest tripDetailRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tripEntity = await _context.Trips.FindAsync(tripDetailRequest.TripId);
            var expenseTypeEntity = await _context.ExpenseTypes.FindAsync(tripDetailRequest.ExpenseId);

            if (tripEntity == null && expenseTypeEntity == null)
            {
                return BadRequest("Trip doesn't exists.");
            }

            string picturePath = string.Empty;
            if (tripDetailRequest.PictureArray != null && tripDetailRequest.PictureArray.Length > 0)
            {
                picturePath = _imageHelper.UploadImage(tripDetailRequest.PictureArray, "Users");
            }

            TripDetailEntity tripDetailEntity = new TripDetailEntity
            {
                Date = tripDetailRequest.Date,
                Description = tripDetailRequest.Description,
                Amount = tripDetailRequest.Amount,
                PicturePath = picturePath,
                Trip = tripEntity,
                Expense = expenseTypeEntity
            };

            _context.TripDetails.Add(tripDetailEntity);
            await _context.SaveChangesAsync();
            return Ok(_converterHelper.ToTripDetailResponse(tripDetailEntity));
        }

        [HttpGet("GetMyTrips/{id}")]
        public async Task<IActionResult> GetMyTrips([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tripEntities = await _context.Trips
                .Include(t => t.Employee)
                .Include(t => t.City)
                .Include(t => t.TripDetails)
                .ThenInclude(t => t.Expense)
                .Where(o => o.Employee.Id == id)
                .OrderByDescending(t => t.StartDate)
                .ToListAsync(); ;

            return Ok(_converterHelper.ToTripResponse(tripEntities));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripEntity([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TripEntity tripEntity = await _context.Trips
                .Include(t => t.City)
                .Include(t => t.TripDetails)
                .ThenInclude(t => t.Expense)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tripEntity == null)
            {
                return BadRequest("Trip not found.");
            }

            return Ok(_converterHelper.ToTripResponse(tripEntity));
        }
    }
}
