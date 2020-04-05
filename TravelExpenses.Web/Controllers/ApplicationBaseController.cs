using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TravelExpenses.Web.Data;

namespace TravelExpenses.Web.Controllers
{
    public class ApplicationBaseController : Controller
    {
        private readonly DataContext _context;

        public ApplicationBaseController(DataContext context)
        {
            _context = context;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (User != null)
            {
                var username = User.Identity.Name;

                if (!string.IsNullOrEmpty(username))
                {
                    var user = _context.Users.SingleOrDefault(u => u.UserName == username);
                    ViewData.Add("FullName", user.FullName);
                    ViewData.Add("PicturePath", user.PicturePath);
                    ViewData.Add("UserType", user.UserType);
                }
            }
            base.OnActionExecuted(filterContext);
        }
    }
}
