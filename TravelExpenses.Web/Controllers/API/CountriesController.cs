using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelExpenses.Web.Data;
using TravelExpenses.Web.Data.Entities;
using TravelExpenses.Web.Interfaces;

namespace TravelExpenses.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CountriesController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IConverterHelper _converterHelper;

        public CountriesController(DataContext dataContext,
            IConverterHelper converterHelper)
        {
            _dataContext = dataContext;
            _converterHelper = converterHelper;
        }

        [HttpGet]
        [Route("GetCountries")]
        public async Task<ActionResult<CountryEntity>> GetCountries()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var countries = await _dataContext.Countries.OrderBy(c => c.Name).ToListAsync();
            return Ok(countries);
        }

        [HttpGet]
        [Route("GetCities")]
        public async Task<ActionResult<CityEntity>> GetCities(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cities = await _dataContext.Cities.Where(t => t.Country.Id == id).ToListAsync();
            return Ok(cities);
        }
    }
}
