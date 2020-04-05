using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelExpenses.Web.Data;

namespace TravelExpenses.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TripsController : ApplicationBaseController
    {
        private readonly DataContext _context;

        public TripsController(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Trips.ToListAsync());
        }
    }
}
