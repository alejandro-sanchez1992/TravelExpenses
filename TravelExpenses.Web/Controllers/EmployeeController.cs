using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelExpenses.Common.Enums;
using TravelExpenses.Web.Data;
using TravelExpenses.Web.Data.Entities;
using TravelExpenses.Web.Interfaces;
using TravelExpenses.Web.Models.Account;

namespace TravelExpenses.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : ApplicationBaseController
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;

        public EmployeeController(
            DataContext context,
            IUserHelper userHelper,
            IImageHelper imageHelper,
            IMailHelper mailHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
        }

        public IActionResult Index()
        {
            return View(_context.Employees
                .Include(o => o.User)
                .Include(o => o.Trips));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Trips)
                .ThenInclude(e => e.City)
                .Include(e => e.Trips)
                .ThenInclude(e => e.TripDetails)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        public async Task<IActionResult> DetailsTrip(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.City)
                .Include(t => t.TripDetails)
                .ThenInclude(t => t.Expense)
                .FirstOrDefaultAsync(o => o.Id == id.Value);

            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new UserEntity
                {
                    Address = model.Address,
                    Document = model.Document,
                    Email = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.Username
                };

                var response = await _userHelper.AddUserAsync(user, model.Password);
                if (response.Succeeded)
                {
                    var userInDB = await _userHelper.GetUserAsync(model.Username);
                    await _userHelper.AddUserToRoleAsync(userInDB, UserType.Employee.ToString());

                    var employee = new EmployeeEntity
                    {
                        Trips = new List<TripEntity>(),
                        User = userInDB
                    };

                    _context.Employees.Add(employee);

                    try
                    {
                        await _context.SaveChangesAsync();

                        var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                        var tokenLink = Url.Action("ConfirmEmail", "Account", new
                        {
                            userid = user.Id,
                            token = myToken
                        }, protocol: HttpContext.Request.Scheme);

                        _mailHelper.SendMail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                            $"To allow the user, " +
                            $"plase click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");

                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, ex.ToString());
                        return View(model);
                    }
                }

                ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Address = employee.User.Address,
                Document = employee.User.Document,
                FirstName = employee.User.FirstName,
                Id = employee.Id,
                LastName = employee.User.LastName,
                PhoneNumber = employee.User.PhoneNumber,
                PicturePath = employee.User.PicturePath
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                string path = model.PicturePath;

                if (model.PictureFile != null)
                {
                    path = await _imageHelper.UploadImageAsync(model.PictureFile, "Users");
                }

                var employee = await _context.Employees
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == model.Id);

                employee.User.Document = model.Document;
                employee.User.FirstName = model.FirstName;
                employee.User.LastName = model.LastName;
                employee.User.Address = model.Address;
                employee.User.PhoneNumber = model.PhoneNumber;
                employee.User.PicturePath = path;

                await _userHelper.UpdateUserAsync(employee.User);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(o => o.User)
                .Include(o => o.Trips)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            if (employee.Trips.Count > 0)
            {
                return RedirectToAction(nameof(Index));
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
