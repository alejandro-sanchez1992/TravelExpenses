using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelExpenses.Web.Data;
using TravelExpenses.Web.Data.Entities;

namespace TravelExpenses.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExpenseTypeController : ControllerBase
    {
        private readonly DataContext _context;

        public ExpenseTypeController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<ExpenseTypeEntity> GetPetTypes()
        {
            return _context.ExpenseTypes.OrderBy(pt => pt.Name);
        }
    }
}
