using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelExpenses.Web.Data;
using TravelExpenses.Web.Models;

namespace TravelExpenses.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeController : ApplicationBaseController
    {
        public HomeController(DataContext context) : base(context) { }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }
    }
}
